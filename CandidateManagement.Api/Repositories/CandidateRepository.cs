using CandidateManagement.Api.Data;
using CandidateManagement.Api.DTOs.Candidates;
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

        public async Task<(List<Candidate> Candidates, int TotalCount)> GetPagedAsync(CandidateListQueryDto query)
        {
            var candidatesQuery = _context.Candidates
                .Include(c => c.CandidateSkills)
                .ThenInclude(cs => cs.Skill)
                .OrderBy(c => c.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var trimmedSearch = query.Search.Trim();
                candidatesQuery = candidatesQuery.Where(c =>
                    EF.Functions.ILike(c.FullName, $"%{trimmedSearch}%"));
            }

            if (query.SkillId.HasValue)
            {
                candidatesQuery = candidatesQuery.Where(c =>
                    c.CandidateSkills.Any(cs => cs.SkillId == query.SkillId.Value));
            }

            var totalCount = await candidatesQuery.CountAsync();

            var candidates = await candidatesQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
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
