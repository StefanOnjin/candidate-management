using CandidateManagement.Api.DTOs.Candidates;
using CandidateManagement.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CandidateManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidatesController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var candidates = await _candidateService.GetPagedAsync(page, pageSize);

            return Ok(candidates);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CandidateResponseDto>> GetById(int id)
        {
            var candidate = await _candidateService.GetByIdAsync(id);

            if (candidate == null)
                return NotFound($"Candidate with id {id} not found.");

            return Ok(candidate);
        }

        [HttpPost]
        public async Task<ActionResult<CandidateResponseDto>> Create(CreateCandidateDto dto)
        {
            try
            {
                var createdCandidate = await _candidateService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdCandidate.Id },
                    createdCandidate
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CandidateResponseDto>> Update(int id, UpdateCandidateDto dto)
        {
            try
            {
                var updatedCandidate = await _candidateService.UpdateAsync(id, dto);

                if (updatedCandidate == null)
                    return NotFound($"Candidate with id {id} not found.");

                return Ok(updatedCandidate);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _candidateService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Candidate with id {id} not found.");

            return NoContent();
        }

        [HttpPost("search")]
        public async Task<ActionResult<List<CandidateResponseDto>>> Search(CandidateSearchDto dto)
        {
            var candidates = await _candidateService.SearchAsync(dto);

            return Ok(candidates);
        }

        [HttpDelete("{candidateId}/skills/{skillId}")]
        public async Task<IActionResult> RemoveSkillFromCandidate(int candidateId, int skillId)
        {
            var removed = await _candidateService.RemoveSkillFromCandidateAsync(candidateId, skillId);

            if (!removed)
                return NotFound("Candidate or skill relation not found.");

            return NoContent();
        }
    }
}
