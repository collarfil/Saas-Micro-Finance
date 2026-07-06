using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var whatsApp = await _unitOfWork.WhatsApps.GetByIdAsync(id);
            if (whatsApp == null) return NotFound();
            return Ok(whatsApp);
        }
        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert(WhatsAppDto dto)
        {
            if (dto.Id == 0)
            {
                var whatsApp = new WhatsApp()
                {
                   
                    CustomerId = dto.CustomerId,
                    Phone = dto.Phone,
                    TemplateName = dto.TemplateName,
                    Payload = dto.Payload,
                    Status = WhatsAppStatus.Sent,
                    SentAt = dto.SentAt
                };
                await _unitOfWork.WhatsApps.AddAsync(whatsApp);
            }
            else
            {
                var whatsApp = await _unitOfWork.WhatsApps.GetFirstOrDefaultAsync(c => c.Id == dto.Id);
                if (whatsApp == null) return NotFound();
                
                whatsApp.CustomerId = dto.CustomerId;
                whatsApp.Phone = dto.Phone;
                whatsApp.TemplateName = dto.TemplateName;
                whatsApp.Payload = dto.Payload;
                whatsApp.Status = WhatsAppStatus.Sent;
                whatsApp.SentAt = dto.SentAt;
                _unitOfWork.WhatsApps.Update(whatsApp);
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var whatsApp = await _unitOfWork.WhatsApps.GetByIdAsync(id);
            if (whatsApp == null) return NotFound();
            _unitOfWork.WhatsApps.Remove(whatsApp);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
