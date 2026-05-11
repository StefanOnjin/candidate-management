using CandidateManagement.Api.DTOs.Skills;

namespace CandidateManagement.Api.DTOs.Candidates
{
    public class CandidateResponseDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        public string ContactNumber { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public List<SkillResponseDto> Skills { get; set; } = new();
    }
}
