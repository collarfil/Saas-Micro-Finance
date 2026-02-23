using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Utility.Services;

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
        public async Task<IActionResult> Apply(Loan dto)
        {
            var result = await _loanService.ApplyLoanAsync(
                dto.TenantId,
                dto.CustomerId,
                dto.LoanProductId,
                dto.Principal);

            return Ok(result);
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
