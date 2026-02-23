using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using System.Threading.Tasks;
using CustomerStatus = Saas_Micro_Finance.Models.CustomerStatus;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            return Ok(customers);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _unitOfWork.Customers.GetFirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }
        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(CustomerDto dto)
        {
            if (dto.Id == 0)
            {
                var customer=new Customer
                {
                    TenantId = dto.TenantId,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Gender =dto.Gender,
                    DOB = dto.DOB,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Created_At = dto.Created_At,
                    Status=CustomerStatus.Active 
                };
                await _unitOfWork.Customers.AddAsync(customer);
            }
            else
            {
                var customer = await _unitOfWork.Customers.GetFirstOrDefaultAsync(c => c.Id == dto.Id);
                if (customer == null)
                {
                    return NotFound();
                    customer.TenantId = dto.TenantId;
                    customer.FirstName = dto.FirstName;
                    customer.LastName = dto.LastName;
                    customer.Gender = dto.Gender;
                    customer.DOB= dto.DOB;
                    customer.Email = dto.Email;
                    customer.Phone = dto.Phone;
                    customer.Created_At = dto.Created_At;
                    customer.Status = dto.Status; // FIX: Update status from DTO
                }

                _unitOfWork.Customers.Update(customer);
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _unitOfWork.Customers.GetFirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            _unitOfWork.Customers.Remove(customer);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}