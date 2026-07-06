using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
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
    }
}
