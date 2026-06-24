using System.Text;
using System.Text.Json;
using CandidateManagement.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CandidateManagement.Api.Messaging
{
    public class RabbitMqActivityEventPublisher : IActivityEventPublisher
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqActivityEventPublisher> _logger;

        public RabbitMqActivityEventPublisher(
            IOptions<RabbitMqOptions> options,
            ILogger<RabbitMqActivityEventPublisher> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public Task PublishAsync(ActivityEvent activityEvent)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _options.HostName,
                    Port = _options.Port,
                    UserName = _options.UserName,
                    Password = _options.Password
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.ExchangeDeclare(
                    exchange: _options.ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false);

                var routingKey = GetRoutingKey(activityEvent.EventType);
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(activityEvent));

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.ContentType = "application/json";

                channel.BasicPublish(
                    exchange: _options.ExchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: body);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to publish activity event {EventType}.", activityEvent.EventType);
            }

            return Task.CompletedTask;
        }

        private static string GetRoutingKey(string eventType)
        {
            return eventType switch
            {
                ActivityEventTypes.CandidateCreated => ActivityRoutingKeys.CandidateCreated,
                ActivityEventTypes.CandidateUpdated => ActivityRoutingKeys.CandidateUpdated,
                ActivityEventTypes.CandidateDeleted => ActivityRoutingKeys.CandidateDeleted,
                ActivityEventTypes.CandidateSkillRemoved => ActivityRoutingKeys.CandidateSkillRemoved,
                ActivityEventTypes.SkillCreated => ActivityRoutingKeys.SkillCreated,
                ActivityEventTypes.SkillUpdated => ActivityRoutingKeys.SkillUpdated,
                ActivityEventTypes.SkillDeleted => ActivityRoutingKeys.SkillDeleted,
                _ => throw new InvalidOperationException($"Unknown activity event type: {eventType}")
            };
        }
    }
}
