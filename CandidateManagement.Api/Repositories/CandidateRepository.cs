using CandidateManagement.Api.Data;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CandidateManagement.Api.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly AppDbContext _context;

        public CandidateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Candidate>> GetAllAsync()
        {
            return await _context.Candidates
                .Include(c => c.CandidateSkills)
                .ThenInclude(cs => cs.Skill)
                .ToListAsync();
        }

        public async Task<(List<Candidate> Candidates, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Candidates
                .Include(c => c.CandidateSkills)
                .ThenInclude(cs => cs.Skill)
                .OrderBy(c => c.Id)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var candidates = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (candidates, totalCount);
        }

        public async Task<Candidate?> GetByIdAsync(int id)
        {
            return await _context.Candidates
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Candidate?> GetByIdWithSkillsAsync(int id)
        {
            return await _context.Candidates
                .Include(c => c.CandidateSkills)
                .ThenInclude(cs => cs.Skill)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string emailAddress)
        {
            return await _context.Candidates
                .AnyAsync(c => c.EmailAddress == emailAddress);
        }

        public async Task<List<Candidate>> SearchAsync(string? fullName, List<int>? skillIds)
        {
            var candidates = _context.Candidates
                .Include(c => c.CandidateSkills)
                .ThenInclude(cs => cs.Skill)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                candidates = candidates.Where(c =>
                    EF.Functions.ILike(c.FullName, $"%{fullName}%"));
            }

            if (skillIds != null && skillIds.Count > 0)
            {
                candidates = candidates.Where(c =>
                    c.CandidateSkills.Any(cs => skillIds.Contains(cs.SkillId)));
            }

            return await candidates.ToListAsync();
        }

        public async Task AddAsync(Candidate candidate)
        {
            await _context.Candidates.AddAsync(candidate);
        }

        public void Update(Candidate candidate)
        {
            _context.Candidates.Update(candidate);
        }

        public void Delete(Candidate candidate)
        {
            _context.Candidates.Remove(candidate);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
