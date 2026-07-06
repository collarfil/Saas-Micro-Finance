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
    public class Customer_KYCController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public Customer_KYCController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customer_KYCs = await _unitOfWork.CustomerKYCs.GetAllAsync();
            return Ok(customer_KYCs);
        }

        [HttpGet("{ById}")]
        public async Task<IActionResult> GetById(int ById)
        {
            var customer_KYC = await _unitOfWork.CustomerKYCs.GetFirstOrDefaultAsync(c => c.Id == ById);
            if (customer_KYC == null)
            {
                return NotFound();
            }
            return Ok(customer_KYC);

        }
        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(Customer_KYCDto dto)
        {
            if (dto.Id == 0)
            {
                var customer_KYC = new Customer_KYC
                {
                    CustomerId = dto.CustomerId,
                    NIN = dto.NIN,
                    BVN = dto.BVN,
                    ID_Type = dto.ID_Type,
                    ID_Number = dto.ID_Number,
                    Passport = dto.Passport,
                    Verfied_At = dto.Verfied_At
                };
                await _unitOfWork.CustomerKYCs.AddAsync(customer_KYC);
            }
            else
            {
                var customer_KYC = await _unitOfWork.CustomerKYCs.GetFirstOrDefaultAsync(c => c.Id == dto.Id);
                if (customer_KYC == null)
                {
                    return NotFound();
                }
                customer_KYC.CustomerId = dto.CustomerId;
                customer_KYC.NIN = dto.NIN;
                customer_KYC.BVN = dto.BVN;
                customer_KYC.ID_Type = dto.ID_Type;
                customer_KYC.ID_Number = dto.ID_Number;
                customer_KYC.Passport = dto.Passport;
                customer_KYC.Verfied_At = dto.Verfied_At;
                _unitOfWork.CustomerKYCs.Update(customer_KYC);
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpDelete("{ById}")]
        public async Task<IActionResult> Delete(int ById)
        {
            var customer_KYC = await _unitOfWork.CustomerKYCs.GetFirstOrDefaultAsync(c => c.Id == ById);
            if (customer_KYC == null)
            {
                return NotFound();
            }
            _unitOfWork.CustomerKYCs.Remove(customer_KYC);
            await _unitOfWork.SaveAsync();
            return NoContent();

        }
    }
}