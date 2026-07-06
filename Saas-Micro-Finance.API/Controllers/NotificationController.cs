using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationController> _logger;
        public NotificationController(IUnitOfWork unitOfWork, ILogger<NotificationController> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _unitOfWork.Notifications.GetAllAsync();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (notification == null) return NotFound();
            return Ok(notification);
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert(NotificationDto dto)
        {
            if (dto.Id == 0)
            {
                var notification = new Notification()
                {
                    
                    Title = dto.Title,
                    Message = dto.Message,
                    Channel = dto.Channel,
                    SentAt = DateTime.UtcNow
                };
                await _unitOfWork.Notifications.AddAsync(notification);
            }
            else
            {
                var notification = await _unitOfWork.Notifications.GetFirstOrDefaultAsync(c => c.Id == dto.Id);
                if (notification == null) return NotFound();
                
                notification.Title = dto.Title;
                notification.Message = dto.Message;
                notification.Channel = dto.Channel;
                _unitOfWork.Notifications.Update(notification);
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (notification == null) return NotFound();
            _unitOfWork.Notifications.Remove(notification);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
