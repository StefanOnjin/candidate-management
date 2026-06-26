using CandidateManagement.Api.DTOs.Activities;
using CandidateManagement.Api.DTOs.Common;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;
using CandidateManagement.Api.Services.Interfaces;
using CandidateManagement.Messaging;

namespace CandidateManagement.Api.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;

        public ActivityLogService(IActivityLogRepository activityLogRepository)
        {
            _activityLogRepository = activityLogRepository;
        }

        public async Task<PagedResultDto<ActivityLogResponseDto>> GetPagedAsync(ActivityLogQueryDto query)
        {
            var safePage = query.Page < 1 ? 1 : query.Page;
            var safePageSize = query.PageSize < 1 ? 10 : query.PageSize;
            var maxPageSize = 50;

            if (safePageSize > maxPageSize)
                safePageSize = maxPageSize;

            var normalizedQuery = new ActivityLogQueryDto
            {
                Search = query.Search,
                EventType = query.EventType,
                EntityType = query.EntityType,
                Page = safePage,
                PageSize = safePageSize
            };

            var (activityLogs, totalCount) = await _activityLogRepository.GetPagedAsync(normalizedQuery);

            return new PagedResultDto<ActivityLogResponseDto>
            {
                Items = activityLogs.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                Page = safePage,
                PageSize = safePageSize
            };
        }

        public async Task SaveActivityLogAsync(ActivityEvent activityEvent)
        {
            var activityLog = new ActivityLog
            {
                EventId = activityEvent.EventId,
                EventType = activityEvent.EventType,
                EntityType = activityEvent.EntityType,
                EntityId = activityEvent.EntityId,
                EntityName = activityEvent.EntityName,
                Message = activityEvent.Message,
                OccurredAtUtc = activityEvent.OccurredAtUtc
            };

            await _activityLogRepository.AddAsync(activityLog);
            await _activityLogRepository.SaveChangesAsync();
        }

        private static ActivityLogResponseDto MapToResponseDto(ActivityLog activityLog)
        {
            return new ActivityLogResponseDto
            {
                Id = activityLog.Id,
                EventId = activityLog.EventId,
                EventType = activityLog.EventType,
                EntityType = activityLog.EntityType,
                EntityId = activityLog.EntityId,
                EntityName = activityLog.EntityName,
                Message = activityLog.Message,
                OccurredAtUtc = activityLog.OccurredAtUtc
            };
        }
    }
}
