using CandidateManagement.Api.DTOs.Skills;
using CandidateManagement.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CandidateManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SkillResponseDto>>> GetAll()
        {
            var skills = await _skillService.GetAllAsync();

            return Ok(skills);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SkillResponseDto>> GetById(int id)
        {
            var skill = await _skillService.GetByIdAsync(id);

            if (skill == null)
                return NotFound($"Skill with id {id} not found.");

            return Ok(skill);
        }

        [HttpPost]
        public async Task<ActionResult<SkillResponseDto>> Create(CreateSkillDto dto)
        {
            try
            {
                var createdSkill = await _skillService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdSkill.Id },
                    createdSkill
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SkillResponseDto>> Update(int id, CreateSkillDto dto)
        {
            try
            {
                var updatedSkill = await _skillService.UpdateAsync(id, dto);

                if (updatedSkill == null)
                    return NotFound($"Skill with id {id} was not found.");

                return Ok(updatedSkill);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _skillService.DeleteAsync(id);

                if (!deleted)
                    return NotFound($"Skill with id {id} was not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); 
            }
        }
    }
}