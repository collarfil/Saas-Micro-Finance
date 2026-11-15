using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.Models.DTOs;
using Saas_Micro_Finance.Utility.Services.Interface;

namespace Saas_Micro_Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "TenantAdmin,BranchManager")]  // tenant-scoped only — SuperAdmin has no tenant DB
    public class GLAccountController : ControllerBase
    {
        private readonly IGLAccountService _service;

        public GLAccountController(IGLAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var accounts = await _service.GetAllAsync(includeInactive);
            return Ok(accounts);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = await _service.GetByIdAsync(id);
            if (account == null) return NotFound(new { message = $"GL Account {id} not found." });
            return Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GLAccountCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GLAccountUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null) return NotFound(new { message = $"GL Account {id} not found." });
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Soft delete — sets IsActive = false.
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"GL Account {id} not found." });
            return NoContent();
        }

        [HttpDelete("{id:int}/hard")]
        public async Task<IActionResult> HardDelete(int id)
        {
            // Guarded hard delete — blocked if account has children or journal lines.
            try
            {
                var deleted = await _service.HardDeleteAsync(id);
                if (!deleted) return NotFound(new { message = $"GL Account {id} not found." });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}