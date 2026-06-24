using System.Text;
using System.Text.Json;
using CandidateManagement.Messaging;
using CandidateManagement.Notifications.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CandidateManagement.Notifications.Messaging
{
    public class RabbitMqActivityEventConsumer : BackgroundService
    {
        private readonly RabbitMqOptions _options;
        private readonly IHubContext<ActivityHub> _hubContext;
        private readonly ILogger<RabbitMqActivityEventConsumer> _logger;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMqActivityEventConsumer(
            IOptions<RabbitMqOptions> options,
            IHubContext<ActivityHub> hubContext,
            ILogger<RabbitMqActivityEventConsumer> logger)
        {
            _options = options.Value;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            InitializeRabbitMq();

            var channel = _channel ?? throw new InvalidOperationException("RabbitMQ channel was not initialized.");
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (_, eventArgs) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                    var activityEvent = JsonSerializer.Deserialize<ActivityEvent>(json);

                    if (activityEvent == null)
                    {
                        _logger.LogWarning("Received empty or invalid activity event.");
                        channel.BasicReject(eventArgs.DeliveryTag, requeue: false);
                        return;
                    }

                    await _hubContext.Clients.All.SendAsync(
                        "ActivityReceived",
                        activityEvent,
                        stoppingToken);

                    channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process activity event.");
                    channel.BasicReject(eventArgs.DeliveryTag, requeue: true);
                }
            };

            channel.BasicConsume(
                queue: _options.QueueName,
                autoAck: false,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }

        private void InitializeRabbitMq()
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            _channel.QueueDeclare(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            BindQueue(ActivityRoutingKeys.CandidateCreated);
            BindQueue(ActivityRoutingKeys.CandidateUpdated);
            BindQueue(ActivityRoutingKeys.CandidateDeleted);
            BindQueue(ActivityRoutingKeys.CandidateSkillRemoved);
            BindQueue(ActivityRoutingKeys.SkillCreated);
            BindQueue(ActivityRoutingKeys.SkillUpdated);
            BindQueue(ActivityRoutingKeys.SkillDeleted);

            _channel.BasicQos(
                prefetchSize: 0,
                prefetchCount: 10,
                global: false);

        }

        private void BindQueue(string routingKey)
        {
            _channel!.QueueBind(
                queue: _options.QueueName,
                exchange: _options.ExchangeName,
                routingKey: routingKey);
        }
    }
}
