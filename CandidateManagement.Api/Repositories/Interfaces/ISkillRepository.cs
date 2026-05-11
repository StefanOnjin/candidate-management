using CandidateManagement.Api.Models;

namespace CandidateManagement.Api.Repositories.Interfaces
{
    public interface ISkillRepository
    {
        Task<List<Skill>> GetAllAsync();

        Task<Skill?> GetByIdAsync(int id);

        Task<List<Skill>> GetByIdsAsync(List<int> ids);

        Task<bool> SkillNameExistsAsync(string skillName);

        Task AddAsync(Skill skill);

        void Update(Skill skill);

        void Delete(Skill skill);

        Task SaveChangesAsync();
        Task<bool> IsSkillAssignedToAnyCandidateAsync(int skillId);
    }
}
