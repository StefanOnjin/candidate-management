using CandidateManagement.Api.Services.Interfaces;
using CandidateManagement.Api.DTOs.Skills;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;

namespace CandidateManagement.Api.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;

        public SkillService(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<List<SkillResponseDto>> GetAllAsync()
        {
            var skills = await _skillRepository.GetAllAsync();

            return skills.Select(MapToResponseDto).ToList();
        }

        public async Task<SkillResponseDto?> GetByIdAsync(int id)
        {
            var skill = await _skillRepository.GetByIdAsync(id);

            if (skill == null)
                return null;

            return MapToResponseDto(skill);
        }

        public async Task<SkillResponseDto> CreateAsync(CreateSkillDto dto)
        {
            var skillName = dto.SkillName.Trim();

            var skillExists = await _skillRepository.SkillNameExistsAsync(skillName);

            if (skillExists)
                throw new InvalidOperationException("Skill with this name already exists.");

            var skill = new Skill
            {
                SkillName = skillName
            };

            await _skillRepository.AddAsync(skill);
            await _skillRepository.SaveChangesAsync();

            return MapToResponseDto(skill);
        }

        public async Task<SkillResponseDto?> UpdateAsync(int id, CreateSkillDto dto)
        {
            var skill = await _skillRepository.GetByIdAsync(id);

            if (skill == null)
                return null;

            var skillName = dto.SkillName.Trim();

            var allSkills = await _skillRepository.GetAllAsync();

            var skillNameUsedByAnotherSkill = allSkills.Any(s =>
                s.Id != id &&
                s.SkillName.ToLower() == skillName.ToLower());

            if (skillNameUsedByAnotherSkill)
                throw new InvalidOperationException("Skill with this name already exists.");

            skill.SkillName = skillName;

            _skillRepository.Update(skill);
            await _skillRepository.SaveChangesAsync();

            return MapToResponseDto(skill);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var skill = await _skillRepository.GetByIdAsync(id);

            if (skill == null)
                return false;

            var isItAssigned = await _skillRepository.IsSkillAssignedToAnyCandidateAsync(id);

            if (isItAssigned)
                throw new InvalidOperationException("Skill cannot be deleted because it is asigned to one or more candidates.");

            _skillRepository.Delete(skill);
            await _skillRepository.SaveChangesAsync();

            return true;
        }

        private static SkillResponseDto MapToResponseDto(Skill skill)
        {
            return new SkillResponseDto
            {
                Id = skill.Id,
                SkillName = skill.SkillName
            };
        }
    }
}
