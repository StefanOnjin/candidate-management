namespace CandidateManagement.Api.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();

    }
}
