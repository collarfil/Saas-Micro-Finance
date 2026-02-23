using System.Diagnostics.Metrics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Customer_AddressController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public Customer_AddressController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customer_Addresses = await _unitOfWork.CustomerAddresses.GetAllAsync();
            return Ok(customer_Addresses);
        }
         [HttpGet("{id}")]
         public async Task<IActionResult> GetById(int id)
        {
                var customer_Address = await _unitOfWork.CustomerAddresses.GetFirstOrDefaultAsync(c => c.Id == id);
                if (customer_Address == null)
                {
                    return NotFound();
                }
                return Ok(customer_Address);
        }
        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(Customer_AddressDto dto)
        {
            if (dto.Id == 0)
            {
                var customer_Address = new Customer_Address
                {
                    TenantId = dto.TenantId,
                    CustomerId = dto.CustomerId,
                    Address = dto.Address,
                    City = dto.City,
                    State = dto.State,
                    Country = dto.Country
                };
                await _unitOfWork.CustomerAddresses.AddAsync(customer_Address);
            }
            else
            {
                var customer_Address = await _unitOfWork.CustomerAddresses.GetFirstOrDefaultAsync(c => c.Id == dto.Id);
                if (customer_Address == null)
                {
                    return NotFound();
                }

                customer_Address.TenantId = dto.TenantId;
                customer_Address.CustomerId = dto.CustomerId;
                customer_Address.Address = dto.Address;
                customer_Address.City = dto.City;
                customer_Address.State = dto.State;
                customer_Address.Country = dto.Country;

                _unitOfWork.CustomerAddresses.Update(customer_Address);
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer_Address = await _unitOfWork.CustomerAddresses.GetFirstOrDefaultAsync(c => c.Id == id);
            if (customer_Address == null)
            {
                return NotFound();
            }
            _unitOfWork.CustomerAddresses.Remove(customer_Address);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
