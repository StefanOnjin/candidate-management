using System.Text.Json;
using CandidateManagement.Api.Messaging;
using CandidateManagement.Api.Repositories.Interfaces;
using CandidateManagement.Messaging;

namespace CandidateManagement.Api.Services
{
    public class OutboxProcessorBackgroundService : BackgroundService
    {
        private const int BatchSize = 20;
        private static readonly TimeSpan PollingDelay = TimeSpan.FromSeconds(5);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxProcessorBackgroundService> _logger;

        public OutboxProcessorBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxProcessorBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingMessagesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed while processing outbox messages.");
                }

                await Task.Delay(PollingDelay, stoppingToken);
            }
        }

        private async Task ProcessPendingMessagesAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
            var activityEventPublisher = scope.ServiceProvider.GetRequiredService<IActivityEventPublisher>();

            var pendingMessages = await outboxRepository.GetPendingAsync(BatchSize);

            foreach (var outboxMessage in pendingMessages)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                try
                {
                    var activityEvent = JsonSerializer.Deserialize<ActivityEvent>(outboxMessage.Payload);

                    if (activityEvent == null)
                        throw new InvalidOperationException(
                            $"Outbox message {outboxMessage.Id} payload could not be deserialized.");

                    await activityEventPublisher.PublishAsync(activityEvent);

                    outboxMessage.ProcessedAtUtc = DateTime.UtcNow;
                    outboxMessage.Error = null;
                    outboxMessage.RetryCount = 0;
                }
                catch (Exception ex)
                {
                    outboxMessage.RetryCount++;
                    outboxMessage.Error = ex.Message;

                    _logger.LogError(
                        ex,
                        "Failed to publish outbox message with id {OutboxMessageId}.",
                        outboxMessage.Id);
                }

                outboxRepository.Update(outboxMessage);
            }

            await outboxRepository.SaveChangesAsync();
        }
    }
}
