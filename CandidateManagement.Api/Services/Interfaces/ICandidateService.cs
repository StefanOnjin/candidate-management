using CandidateManagement.Api.DTOs.Candidates;
using CandidateManagement.Api.DTOs.Common;

namespace CandidateManagement.Api.Services.Interfaces
{
    public interface ICandidateService
    {
        Task<PagedResultDto<CandidateResponseDto>> GetPagedAsync(int page, int pageSize);

        Task<CandidateResponseDto?> GetByIdAsync(int id);

        Task<CandidateResponseDto> CreateAsync(CreateCandidateDto dto);

        Task<CandidateResponseDto?> UpdateAsync(int id, UpdateCandidateDto dto);

        Task<bool> DeleteAsync(int id);

        Task<List<CandidateResponseDto>> SearchAsync(CandidateSearchDto dto);

        Task<bool> RemoveSkillFromCandidateAsync(int candidateId, int skillId);
    }
}
