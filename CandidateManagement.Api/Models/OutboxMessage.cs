namespace CandidateManagement.Api.Models
{
    public class OutboxMessage
    {
        public int Id { get; set; }
        public Guid EventId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty; 
        public DateTime OccurredAtUtc { get; set; } 
        public DateTime? ProcessedAtUtc { get; set; } 
        public int RetryCount { get; set; } 
        public string? Error { get; set; } 
    }
}
