using CandidateManagement.Api.Data;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CandidateManagement.Api.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly AppDbContext _context;

        public SkillRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Skill>> GetAllAsync()
        {
            return await _context.Skills
                .OrderBy(s => s.SkillName)
                .ToListAsync();
        }

        public async Task<Skill?> GetByIdAsync(int id)
        {
            return await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Skill>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Skills
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }

        public async Task<bool> SkillNameExistsAsync(string skillName)
        {
            return await _context.Skills
                .AnyAsync(s => s.SkillName == skillName);
        }

        public async Task AddAsync(Skill skill)
        {
            await _context.Skills.AddAsync(skill);
        }

        public void Update(Skill skill)
        {
            _context.Skills.Update(skill);
        }

        public void Delete(Skill skill)
        {
            _context.Skills.Remove(skill);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsSkillAssignedToAnyCandidateAsync(int skillId)
        {
            return await _context.CandidateSkills
                .AnyAsync(cs => cs.SkillId == skillId);
        }
    }
}