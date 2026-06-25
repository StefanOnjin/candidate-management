namespace CandidateManagement.Api.DTOs.Candidates
{
    public class CandidateListQueryDto
    {
        public string? Search { get; set; }

        public int? SkillId { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
