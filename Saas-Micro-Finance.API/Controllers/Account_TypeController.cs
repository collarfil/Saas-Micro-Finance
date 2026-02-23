using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Account_TypeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public Account_TypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accountTypes = await _unitOfWork.AccountTypes.GetAllAsync();
            return Ok(accountTypes);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var accountType = await _unitOfWork.AccountTypes.GetFirstOrDefaultAsync(a => a.Id == id);
            if (accountType == null)
            {
                return NotFound();
            }
            return Ok(accountType);
        }
        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(Account_TypeDto dto)
        {
            if (dto.Id == 0)
            {
                var entity = new Account_Type
                {
                    TenantId = dto.TenantId,
                    Name = dto.Name,
                    Minimum_Balance = dto.Minimum_Balance,
                    Interest_Rate = dto.Interest_Rate,
                    Created_At = DateTime.UtcNow
                };

                await _unitOfWork.AccountTypes.AddAsync(entity);
            }
            else
            {
                var entity = await _unitOfWork.AccountTypes.GetByIdAsync(dto.Id);

                if (entity == null)
                    return NotFound();

                entity.Name = dto.Name;
                entity.Minimum_Balance = dto.Minimum_Balance;
                entity.Interest_Rate = dto.Interest_Rate;
            }

            await _unitOfWork.SaveAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var accountType = await _unitOfWork.AccountTypes.GetFirstOrDefaultAsync(a => a.Id == id);
            if (accountType == null)
            {
                return NotFound();
            }
            _unitOfWork.AccountTypes.Remove(accountType);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
