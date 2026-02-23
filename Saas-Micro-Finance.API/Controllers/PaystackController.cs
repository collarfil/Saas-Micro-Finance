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
    public class PaystackPaymentsController : ControllerBase
    {
        private readonly IPaystackService _service;

        public PaystackPaymentsController(IPaystackService service)
        {
            _service = service;
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> Initialize([FromBody] CreatePaymentDto dto)
        {
            var payment = await _service.InitializePaymentAsync(dto);
            return Ok(payment);
        }

        [HttpPost("confirm/{reference}")]
        public async Task<IActionResult> Confirm(string reference, [FromQuery] bool success)
        {
            var payment = await _service.ConfirmPaymentAsync(reference, success);
            if (payment == null) return NotFound();
            return Ok(payment);
        }
    }
}
