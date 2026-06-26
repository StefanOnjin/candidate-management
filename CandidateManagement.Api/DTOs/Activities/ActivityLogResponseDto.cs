namespace CandidateManagement.Api.DTOs.Activities
{
    public class ActivityLogResponseDto
    {
        public int Id { get; set; }

        public Guid EventId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string EntityType { get; set; } = string.Empty;

        public int EntityId { get; set; }

        public string EntityName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime OccurredAtUtc { get; set; }
    }
}
