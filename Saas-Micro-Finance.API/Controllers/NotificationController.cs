using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (notification == null) return NotFound(new { message = $"Notification {id} not found." });
            return Ok(notification);
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert(NotificationDto dto)
        {
            Notification notification;

            if (dto.Id == 0)
            {
                notification = new Notification
                {
                    Title = dto.Title,
                    Message = dto.Message,
                    Channel = dto.Channel,
                    SentAt = dto.SentAt == default ? DateTime.UtcNow : dto.SentAt   // ← respect caller
                };
                await _unitOfWork.Notifications.AddAsync(notification);
            }
            else
            {
                notification = await _unitOfWork.Notifications.GetFirstOrDefaultAsync(c => c.Id == dto.Id);
                if (notification == null) return NotFound(new { message = $"Notification {dto.Id} not found." });

                notification.Title = dto.Title;
                notification.Message = dto.Message;
                notification.Channel = dto.Channel;
                if (dto.SentAt != default) notification.SentAt = dto.SentAt;   // ← was never updated before

                _unitOfWork.Notifications.Update(notification);
            }
            await _unitOfWork.SaveAsync();
            return Ok(notification);   // ← now returns the object, not empty
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (notification == null) return NotFound(new { message = $"Notification {id} not found." });

            _unitOfWork.Notifications.Remove(notification);
            await _unitOfWork.SaveAsync();
            return Ok(new { message = "Notification deleted." });
        }
    }
}