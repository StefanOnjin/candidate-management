using CandidateManagement.Api.DTOs.Skills;

namespace CandidateManagement.Api.Services.Interfaces
{
    public interface ISkillService
    {
        Task<List<SkillResponseDto>> GetAllAsync();

        Task<SkillResponseDto?> GetByIdAsync(int id);

        Task<SkillResponseDto> CreateAsync(CreateSkillDto dto);

        Task<SkillResponseDto?> UpdateAsync(int id, CreateSkillDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
