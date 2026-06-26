using CandidateManagement.Api.DTOs.Activities;
using CandidateManagement.Api.DTOs.Common;
using CandidateManagement.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CandidateManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityLogService _activityLogService;

        public ActivitiesController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<ActivityLogResponseDto>>> GetAll(
            [FromQuery] ActivityLogQueryDto query)
        {
            var activityLogs = await _activityLogService.GetPagedAsync(query);

            return Ok(activityLogs);
        }
    }
}
