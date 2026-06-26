using CandidateManagement.Api.Data;
using CandidateManagement.Api.DTOs.Activities;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CandidateManagement.Api.Repositories
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly AppDbContext _context;

        public ActivityLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<ActivityLog> ActivityLogs, int TotalCount)> GetPagedAsync(ActivityLogQueryDto query)
        {
            var activityLogsQuery = _context.ActivityLogs
                .OrderByDescending(a => a.OccurredAtUtc)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var trimmedSearch = query.Search.Trim();
                activityLogsQuery = activityLogsQuery.Where(a =>
                    EF.Functions.ILike(a.Message, $"%{trimmedSearch}%") ||
                    EF.Functions.ILike(a.EntityName, $"%{trimmedSearch}%"));
            }

            if (!string.IsNullOrWhiteSpace(query.EventType))
            {
                var trimmedEventType = query.EventType.Trim();
                activityLogsQuery = activityLogsQuery.Where(a => a.EventType == trimmedEventType);
            }

            if (!string.IsNullOrWhiteSpace(query.EntityType))
            {
                var trimmedEntityType = query.EntityType.Trim();
                activityLogsQuery = activityLogsQuery.Where(a => a.EntityType == trimmedEntityType);
            }

            var totalCount = await activityLogsQuery.CountAsync();

            var activityLogs = await activityLogsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (activityLogs, totalCount);
        }

        public async Task AddAsync(ActivityLog activityLog)
        {
            await _context.ActivityLogs.AddAsync(activityLog);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
