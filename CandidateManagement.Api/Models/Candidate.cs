namespace CandidateManagement.Api.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty; 
        public DateOnly DateOfBirth { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();

    }
}
