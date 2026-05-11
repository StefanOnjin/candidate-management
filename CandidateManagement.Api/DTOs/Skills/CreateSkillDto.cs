using System.ComponentModel.DataAnnotations;

namespace CandidateManagement.Api.DTOs.Skills
{
    public class CreateSkillDto
    {
        [Required]
        [MaxLength(50)]
        public string SkillName { get; set; } = string.Empty;
    }
}
