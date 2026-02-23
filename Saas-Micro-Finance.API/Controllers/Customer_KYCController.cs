using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
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
        public async Task<IActionResult> Upsert(Customer_KYC customer_KYC)
        {
            if (customer_KYC.Id == 0)
            {
                await _unitOfWork.CustomerKYCs.AddAsync(customer_KYC);
            }
            else
            {
                _unitOfWork.CustomerKYCs.Update(customer_KYC);
            }
            await _unitOfWork.SaveAsync();
            return Ok(customer_KYC);
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