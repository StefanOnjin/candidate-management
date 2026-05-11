namespace CandidateManagement.Api.DTOs.Candidates
{
    public class CandidateSearchDto
    {
        public string? FullName { get; set; }

        public List<int>? SkillIds { get; set; }
    }
}
