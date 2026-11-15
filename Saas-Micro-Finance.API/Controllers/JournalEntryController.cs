using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.Models.DTOs;
using Saas_Micro_Finance.Utility.Services.Interface;

namespace Saas_Micro_Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "TenantAdmin,BranchManager")]
    public class JournalEntryController : ControllerBase
    {
        private readonly IJournalEntryService _service;

        public JournalEntryController(IJournalEntryService service)
        {
            _service = service;
        }

        private string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name ?? "system";

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? sourceModule = null,
            [FromQuery] int? branchId = null)
        {
            var entries = await _service.GetAllAsync(status, fromDate, toDate, sourceModule, branchId);
            return Ok(entries);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entry = await _service.GetByIdAsync(id);
            if (entry == null) return NotFound(new { message = $"Journal Entry {id} not found." });
            return Ok(entry);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JournalEntryCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto, CurrentUserId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/post")]
        public async Task<IActionResult> Post(int id)
        {
            try
            {
                var result = await _service.PostAsync(id, CurrentUserId);
                if (result == null) return NotFound(new { message = $"Journal Entry {id} not found." });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _service.CancelAsync(id);
                if (result == null) return NotFound(new { message = $"Journal Entry {id} not found." });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/reverse")]
        public async Task<IActionResult> Reverse(int id)
        {
            try
            {
                var result = await _service.ReverseAsync(id, CurrentUserId);
                if (result == null) return NotFound(new { message = $"Journal Entry {id} not found." });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("trial-balance")]
        public async Task<IActionResult> TrialBalance(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] int? branchId = null)
        {
            var result = await _service.GetTrialBalanceAsync(fromDate, toDate, branchId);
            return Ok(result);
        }
    }
}