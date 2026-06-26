namespace CandidateManagement.Api.DTOs.Activities
{
    public class ActivityLogQueryDto
    {
        public string? Search { get; set; }

        public string? EventType { get; set; }

        public string? EntityType { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
