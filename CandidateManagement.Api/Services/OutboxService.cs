using System.Text.Json;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;
using CandidateManagement.Api.Services.Interfaces;
using CandidateManagement.Messaging;

namespace CandidateManagement.Api.Services
{
    public class OutboxService : IOutboxService
    {
        private readonly IOutboxRepository _outboxRepository;

        public OutboxService(IOutboxRepository outboxRepository)
        {
            _outboxRepository = outboxRepository;
        }

        public async Task SaveMessageAsync(ActivityEvent activityEvent)
        {
            var outboxMessage = new OutboxMessage
            {
                EventId = activityEvent.EventId,
                EventType = activityEvent.EventType,
                Payload = JsonSerializer.Serialize(activityEvent),
                OccurredAtUtc = activityEvent.OccurredAtUtc,
                RetryCount = 0,
                ProcessedAtUtc = null,
                Error = null
            };

            await _outboxRepository.AddAsync(outboxMessage);
            await _outboxRepository.SaveChangesAsync();
        }
    }
}