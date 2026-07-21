using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public LoanProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var loanProducts = await _unitOfWork.LoanProducts.GetAllAsync();
            return Ok(loanProducts);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var loanProduct = await _unitOfWork.LoanProducts.GetFirstOrDefaultAsync(c => c.Id == id);
            if (loanProduct == null)
            {
                return NotFound();
            }
            return Ok(loanProduct);
        }
        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(LoanProductDto dto)
        {
            if (dto.Id == 0)
            {
                var loanProduct = new LoanProduct
                {
                    Name = dto.Name,
                    InterestRate = dto.InterestRate,
                    Duration = dto.Duration,
                    PenaltyRate = dto.PenaltyRate
                };
                await _unitOfWork.LoanProducts.AddAsync(loanProduct);
            }
            else
            {
                var loanProduct = await _unitOfWork.LoanProducts.GetFirstOrDefaultAsync(c => c.Id == dto.Id);
                if (loanProduct == null)
                {
                    return NotFound();
                }
                loanProduct.Name = dto.Name;
                loanProduct.InterestRate = dto.InterestRate;
                loanProduct.Duration = dto.Duration;
                loanProduct.PenaltyRate = dto.PenaltyRate;
                _unitOfWork.LoanProducts.Update(loanProduct);
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var loanProduct = await _unitOfWork.LoanProducts.GetFirstOrDefaultAsync(c => c.Id == id);
            if (loanProduct == null)
            {
                return NotFound();
            }
            _unitOfWork.LoanProducts.Remove(loanProduct);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}