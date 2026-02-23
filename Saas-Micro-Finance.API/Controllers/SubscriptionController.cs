using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.Models.DTOs;
using Saas_Micro_Finance.Utility.Services;

namespace Saas_Micro_Finance.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _service;

        public SubscriptionsController(ISubscriptionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subs = await _service.GetSubscriptionsAsync();
            return Ok(subs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var sub = await _service.GetByIdAsync(id);
            if (sub == null) return NotFound();
            return Ok(sub);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubscriptionDto dto)
        {
            var created = await _service.CreateSubscriptionAsync(dto);
            return Ok(created);
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _service.CancelSubscriptionAsync(id);
            if (!success) return NotFound();
            return Ok("Subscription canceled");
        }
    }
}
