using System.ComponentModel.DataAnnotations;

namespace CandidateManagement.Api.DTOs.Candidates
{
    public class UpdateCandidateDto
    {
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [MaxLength(30)]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string EmailAddress { get; set; } = string.Empty;

        public List<int> SkillIds { get; set; } = new();
    }
}
