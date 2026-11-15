using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]   // any authenticated user in the tenant
    public class WhatsAppController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public WhatsAppController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var whatsApps = await _unitOfWork.WhatsApps.GetAllAsync();
            return Ok(whatsApps);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var whatsApp = await _unitOfWork.WhatsApps.GetByIdAsync(id);
            if (whatsApp == null) return NotFound(new { message = $"WhatsApp message {id} not found." });
            return Ok(whatsApp);
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert(WhatsAppDto dto)
        {
            WhatsApp whatsApp;

            if (dto.Id == 0)
            {
                whatsApp = new WhatsApp
                {
                    CustomerId = dto.CustomerId,
                    Phone = dto.Phone,
                    TemplateName = dto.TemplateName,
                    Payload = dto.Payload,
                    Status = dto.Status == 0 ? WhatsAppStatus.Sent : dto.Status,   // ← respect caller, default Sent
                    SentAt = dto.SentAt == default ? DateTime.UtcNow : dto.SentAt
                };
                await _unitOfWork.WhatsApps.AddAsync(whatsApp);
            }
            else
            {
                whatsApp = await _unitOfWork.WhatsApps.GetFirstOrDefaultAsync(c => c.Id == dto.Id);
                if (whatsApp == null) return NotFound(new { message = $"WhatsApp message {dto.Id} not found." });

                whatsApp.CustomerId = dto.CustomerId;
                whatsApp.Phone = dto.Phone;
                whatsApp.TemplateName = dto.TemplateName;
                whatsApp.Payload = dto.Payload;
                whatsApp.Status = dto.Status == 0 ? whatsApp.Status : dto.Status;   // ← only override if provided
                if (dto.SentAt != default) whatsApp.SentAt = dto.SentAt;

                _unitOfWork.WhatsApps.Update(whatsApp);
            }
            await _unitOfWork.SaveAsync();
            return Ok(whatsApp);   // ← now returns the object, not empty
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var whatsApp = await _unitOfWork.WhatsApps.GetByIdAsync(id);
            if (whatsApp == null) return NotFound(new { message = $"WhatsApp message {id} not found." });

            _unitOfWork.WhatsApps.Remove(whatsApp);
            await _unitOfWork.SaveAsync();
            return Ok(new { message = "WhatsApp message deleted." });
        }
    }
}