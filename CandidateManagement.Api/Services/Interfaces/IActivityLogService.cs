using CandidateManagement.Api.DTOs.Activities;
using CandidateManagement.Api.DTOs.Common;
using CandidateManagement.Messaging;

namespace CandidateManagement.Api.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task<PagedResultDto<ActivityLogResponseDto>> GetPagedAsync(ActivityLogQueryDto query);

        Task SaveActivityLogAsync(ActivityEvent activityEvent);
    }
}
