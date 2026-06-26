using CandidateManagement.Api.DTOs.Activities;
using CandidateManagement.Api.Models;

namespace CandidateManagement.Api.Repositories.Interfaces
{
    public interface IActivityLogRepository
    {
        Task<(List<ActivityLog> ActivityLogs, int TotalCount)> GetPagedAsync(ActivityLogQueryDto query);

        Task AddAsync(ActivityLog activityLog);

        Task SaveChangesAsync();
    }
}
