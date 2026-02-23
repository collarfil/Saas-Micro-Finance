using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using TenantStatus = Saas_Micro_Finance.Models.TenantStatus;

namespace Saas_Micro_Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TenantController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenants = await _unitOfWork.Tenants.GetAllAsync();
            return Ok(tenants);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tenant = await _unitOfWork.Tenants.GetByIdAsync(id);

            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(TenantDto dto)
        {
            if (dto.Id == 0)
            {
                var tenant = new Tenant
                {
                    name = dto.name,
                    License_Number = dto.License_Number,
                    CBN_Code = dto.CBN_Code,
                    Phone = dto.Phone,
                    Address = dto.Address,
                    Status = TenantStatus.Active,
                    Created_At = DateTime.UtcNow
                };

                await _unitOfWork.Tenants.AddAsync(tenant);
            }
            else
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(dto.Id);

                if (tenant == null)
                    return NotFound();

                tenant.name = dto.name;
                tenant.Phone = dto.Phone;
                tenant.Address = dto.Address;
                tenant.Status = Models.TenantStatus.Active; 
            }

            await _unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tenant = await _unitOfWork.Tenants.GetByIdAsync(id);

            if (tenant == null)
                return NotFound();

            _unitOfWork.Tenants.Remove(tenant);
            await _unitOfWork.SaveAsync();

            return Ok();
        }
    }
}