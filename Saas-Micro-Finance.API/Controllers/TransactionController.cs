using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.Models.DTOs;
using Saas_Micro_Finance.Utility.Services;

namespace Saas_Micro_Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(TransactionDto dto)
        {
            var result = await _transactionService.DepositAsync(
                dto.TenantId,
                dto.AccountId,
                dto.Amount,
                dto.Reference,
                dto.Narration);

            return Ok(result);
        }
    }
}
