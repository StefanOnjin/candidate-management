using CandidateManagement.Api.DTOs.Candidates;
using CandidateManagement.Api.Models;

namespace CandidateManagement.Api.Repositories.Interfaces
{
    public interface ICandidateRepository
    {
        Task<List<Candidate>> GetAllAsync();

        Task<(List<Candidate> Candidates, int TotalCount)> GetPagedAsync(CandidateListQueryDto query);

        Task<Candidate?> GetByIdAsync(int id);

        Task<Candidate?> GetByIdWithSkillsAsync(int id);

        Task<bool> EmailExistsAsync(string emailAddress);

        Task AddAsync(Candidate candidate);

        void Update(Candidate candidate);

        void Delete(Candidate candidate);

        Task SaveChangesAsync();
    }
}
