using CandidateManagement.Api.Messaging;
using CandidateManagement.Api.Services.Interfaces;
using CandidateManagement.Api.DTOs.Candidates;
using CandidateManagement.Api.DTOs.Common;
using CandidateManagement.Api.DTOs.Skills;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;
using CandidateManagement.Messaging;

namespace CandidateManagement.Api.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IActivityEventPublisher _activityEventPublisher;

        public CandidateService(
            ICandidateRepository candidateRepository,
            ISkillRepository skillRepository,
            IActivityEventPublisher activityEventPublisher)
        {
            _candidateRepository = candidateRepository;
            _skillRepository = skillRepository;
            _activityEventPublisher = activityEventPublisher;
        }

        public async Task<PagedResultDto<CandidateResponseDto>> GetPagedAsync(CandidateListQueryDto query)
        {
            var safePage = query.Page < 1 ? 1 : query.Page;
            var safePageSize = query.PageSize < 1 ? 10 : query.PageSize;
            var maxPageSize = 50;

            if (safePageSize > maxPageSize)
                safePageSize = maxPageSize;

            var normalizedQuery = new CandidateListQueryDto
            {
                Search = query.Search,
                SkillId = query.SkillId,
                Page = safePage,
                PageSize = safePageSize
            };

            var (candidates, totalCount) = await _candidateRepository.GetPagedAsync(normalizedQuery);

            return new PagedResultDto<CandidateResponseDto>
            {
                Items = candidates.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                Page = safePage,
                PageSize = safePageSize
            };
        }

        public async Task<CandidateResponseDto?> GetByIdAsync(int id)
        {
            var candidate = await _candidateRepository.GetByIdWithSkillsAsync(id);

            if (candidate == null)
                return null;

            return MapToResponseDto(candidate);
        }

        public async Task<CandidateResponseDto> CreateAsync(CreateCandidateDto dto)
        {
            var normalizedEmail = dto.EmailAddress.Trim().ToLower();

            var emailExists = await _candidateRepository.EmailExistsAsync(normalizedEmail);

            if (emailExists)
                throw new InvalidOperationException("Candidate with this email already exists.");

            var skillIds = dto.SkillIds.Distinct().ToList();
            var skills = await _skillRepository.GetByIdsAsync(skillIds);

            if (skills.Count != skillIds.Count)
                throw new InvalidOperationException("One or more skills do not exist.");

            var candidate = new Candidate
            {
                FullName = dto.FullName.Trim(),
                DateOfBirth = dto.DateOfBirth,
                ContactNumber = dto.ContactNumber.Trim(),
                EmailAddress = normalizedEmail,
                CandidateSkills = skillIds.Select(skillId => new CandidateSkill
                {
                    SkillId = skillId
                }).ToList()
            };

            await _candidateRepository.AddAsync(candidate);
            await _candidateRepository.SaveChangesAsync();

            var createdCandidate = await _candidateRepository.GetByIdWithSkillsAsync(candidate.Id);

            await _activityEventPublisher.PublishAsync(new ActivityEvent
            {
                EventType = ActivityEventTypes.CandidateCreated,
                EntityType = ActivityEntityTypes.Candidate,
                EntityId = createdCandidate!.Id,
                EntityName = createdCandidate.FullName,
                Message = $"New candidate added: {createdCandidate.FullName}"
            });

            return MapToResponseDto(createdCandidate!);
        }

        public async Task<CandidateResponseDto?> UpdateAsync(int id, UpdateCandidateDto dto)
        {
            var candidate = await _candidateRepository.GetByIdWithSkillsAsync(id);

            if (candidate == null)
                return null;

            var normalizedEmail = dto.EmailAddress.Trim().ToLower();

            var allCandidates = await _candidateRepository.GetAllAsync();

            var emailUsedByAnotherCandidate = allCandidates.Any(c =>
                c.Id != id &&
                c.EmailAddress == normalizedEmail);

            if (emailUsedByAnotherCandidate)
                throw new InvalidOperationException("Candidate with this email already exists.");

            var skillIds = dto.SkillIds.Distinct().ToList();
            var skills = await _skillRepository.GetByIdsAsync(skillIds);

            if (skills.Count != skillIds.Count)
                throw new InvalidOperationException("One or more skills do not exist.");

            candidate.FullName = dto.FullName.Trim();
            candidate.DateOfBirth = dto.DateOfBirth;
            candidate.ContactNumber = dto.ContactNumber.Trim();
            candidate.EmailAddress = normalizedEmail;

            candidate.CandidateSkills.Clear();

            foreach (var skillId in skillIds)
            {
                candidate.CandidateSkills.Add(new CandidateSkill
                {
                    CandidateId = candidate.Id,
                    SkillId = skillId
                });
            }

            _candidateRepository.Update(candidate);
            await _candidateRepository.SaveChangesAsync();

            var updatedCandidate = await _candidateRepository.GetByIdWithSkillsAsync(candidate.Id);

            await _activityEventPublisher.PublishAsync(new ActivityEvent
            {
                EventType = ActivityEventTypes.CandidateUpdated,
                EntityType = ActivityEntityTypes.Candidate,
                EntityId = updatedCandidate!.Id,
                EntityName = updatedCandidate.FullName,
                Message = $"Candidate updated: {updatedCandidate.FullName}"
            });

            return MapToResponseDto(updatedCandidate!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var candidate = await _candidateRepository.GetByIdAsync(id);

            if (candidate == null)
                return false;

            var candidateName = candidate.FullName;

            _candidateRepository.Delete(candidate);
            await _candidateRepository.SaveChangesAsync();

            await _activityEventPublisher.PublishAsync(new ActivityEvent
            {
                EventType = ActivityEventTypes.CandidateDeleted,
                EntityType = ActivityEntityTypes.Candidate,
                EntityId = id,
                EntityName = candidateName,
                Message = $"Candidate deleted: {candidateName}"
            });

            return true;
        }

        public async Task<bool> RemoveSkillFromCandidateAsync(int candidateId, int skillId)
        {
            var candidate = await _candidateRepository.GetByIdWithSkillsAsync(candidateId);

            if (candidate == null)
                return false;

            var candidateSkill = candidate.CandidateSkills
                .FirstOrDefault(cs => cs.SkillId == skillId);

            if (candidateSkill == null)
                return false;

            var skillName = candidateSkill.Skill.SkillName;

            candidate.CandidateSkills.Remove(candidateSkill);

            _candidateRepository.Update(candidate);
            await _candidateRepository.SaveChangesAsync();

            await _activityEventPublisher.PublishAsync(new ActivityEvent
            {
                EventType = ActivityEventTypes.CandidateSkillRemoved,
                EntityType = ActivityEntityTypes.Candidate,
                EntityId = candidate.Id,
                EntityName = candidate.FullName,
                Message = $"Skill removed from candidate {candidate.FullName}: {skillName}",
                Metadata = new Dictionary<string, string>
                {
                    ["skillId"] = skillId.ToString(),
                    ["skillName"] = skillName
                }
            });

            return true;
        }

        private static CandidateResponseDto MapToResponseDto(Candidate candidate)
        {
            return new CandidateResponseDto
            {
                Id = candidate.Id,
                FullName = candidate.FullName,
                DateOfBirth = candidate.DateOfBirth,
                ContactNumber = candidate.ContactNumber,
                EmailAddress = candidate.EmailAddress,
                Skills = candidate.CandidateSkills.Select(cs => new SkillResponseDto
                {
                    Id = cs.Skill.Id,
                    SkillName = cs.Skill.SkillName
                }).ToList()
            };
        }
    }
}
