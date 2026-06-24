namespace CandidateManagement.Messaging;

public class ActivityEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();

    public string EventType { get; set; } = string.Empty;

    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

    public string EntityType { get; set; } = string.Empty;

    public int EntityId { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = new();
}
