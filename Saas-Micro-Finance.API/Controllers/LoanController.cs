using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using Saas_Micro_Finance.Utility.Services.Interface;

namespace Saas_Micro_Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly IUnitOfWork _unitOfWork;
        public LoanController(ILoanService loanService, IUnitOfWork unitOfWork)
        {
            _loanService = loanService;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var loans = await _unitOfWork.Loans.GetAllAsync();

                var result = loans.Select(l => new
                {
                    Id = l.Id,
                    CustomerId = l.CustomerId,
                    LoanProductId = l.LoanProductId,
                    Principal = l.Principal,
                    Interest = l.Interest,
                    Status = l.Status.ToString(),
                    DisbursedAt = l.DisbursedAt
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("LOAN GET ERROR: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply(ApplyLoanDto dto)
        {
            await _loanService.ApplyLoanAsync(
                dto.CustomerId,
                dto.LoanProductId,
                dto.Principal);

            return Ok(new
            {
                message = "Loan application submitted."
            });
        }

        [HttpPost("approve/{loanId}")]
        public async Task<IActionResult> Approve(int loanId)
        {
            var result = await _loanService.ApproveLoanAsync(loanId);
            return Ok(result);
        }

        [HttpPost("disburse/{loanId}")]
        public async Task<IActionResult> Disburse(int loanId)
        {
            var result = await _loanService.DisburseLoanAsync(loanId);
            return Ok(result);
        }

        [HttpPost("repay")]
        public async Task<IActionResult> Repay(LoanRepayment dto)
        {
            var result = await _loanService.RepayLoanAsync(dto.LoanId, dto.Amount);
            return Ok(result);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerLoans(int customerId)
        {
            var loans = await _unitOfWork.Loans.GetAllAsync(l => l.CustomerId == customerId);
            return Ok(loans);
        }
    }
}
