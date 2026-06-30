using CandidateManagement.Api.Services.Interfaces;
using CandidateManagement.Api.DTOs.Skills;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;
using CandidateManagement.Messaging;

namespace CandidateManagement.Api.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;
        private readonly IActivityLogService _activityLogService;
        private readonly IOutboxService _outboxService;
        private readonly ITransactionManager _transactionManager;

        public SkillService(
            ISkillRepository skillRepository,
            IActivityLogService activityLogService,
            IOutboxService outboxService,
            ITransactionManager transactionManager)
        {
            _skillRepository = skillRepository;
            _activityLogService = activityLogService;
            _outboxService = outboxService;
            _transactionManager = transactionManager;
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

            return await _transactionManager.ExecuteAsync(async () =>
            {
                var skill = new Skill
                {
                    SkillName = skillName
                };

                await _skillRepository.AddAsync(skill);
                await _skillRepository.SaveChangesAsync();

                var activityEvent = new ActivityEvent
                {
                    EventType = ActivityEventTypes.SkillCreated,
                    EntityType = ActivityEntityTypes.Skill,
                    EntityId = skill.Id,
                    EntityName = skill.SkillName,
                    Message = $"New skill added: {skill.SkillName}"
                };

                await SaveActivityAsync(activityEvent);

                return MapToResponseDto(skill);
            });
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

            return await _transactionManager.ExecuteAsync(async () =>
            {
                skill.SkillName = skillName;

                _skillRepository.Update(skill);
                await _skillRepository.SaveChangesAsync();

                var activityEvent = new ActivityEvent
                {
                    EventType = ActivityEventTypes.SkillUpdated,
                    EntityType = ActivityEntityTypes.Skill,
                    EntityId = skill.Id,
                    EntityName = skill.SkillName,
                    Message = $"Skill updated: {skill.SkillName}"
                };

                await SaveActivityAsync(activityEvent);

                return MapToResponseDto(skill);
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var skill = await _skillRepository.GetByIdAsync(id);

            if (skill == null)
                return false;

            var isItAssigned = await _skillRepository.IsSkillAssignedToAnyCandidateAsync(id);

            if (isItAssigned)
                throw new InvalidOperationException("Skill cannot be deleted because it is asigned to one or more candidates.");

            return await _transactionManager.ExecuteAsync(async () =>
            {
                var skillName = skill.SkillName;

                _skillRepository.Delete(skill);
                await _skillRepository.SaveChangesAsync();

                var activityEvent = new ActivityEvent
                {
                    EventType = ActivityEventTypes.SkillDeleted,
                    EntityType = ActivityEntityTypes.Skill,
                    EntityId = id,
                    EntityName = skillName,
                    Message = $"Skill deleted: {skillName}"
                };

                await SaveActivityAsync(activityEvent);

                return true;
            });
        }

        private async Task SaveActivityAsync(ActivityEvent activityEvent)
        {
            await _activityLogService.SaveActivityLogAsync(activityEvent);
            await _outboxService.SaveMessageAsync(activityEvent);
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
