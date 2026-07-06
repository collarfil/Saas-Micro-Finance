using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.Models.DTOs;
using Saas_Micro_Finance.Utility.Services.Interface;

namespace Saas_Micro_Finance.API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class LedgerController : ControllerBase
    {
        private readonly ILedgerService _ledgerService;

        public LedgerController(ILedgerService ledgerService)
        {
            _ledgerService = ledgerService;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitTransaction(List<LedgerEntryDto> entries)
        {
            var result = await _ledgerService.SubmitTransactionAsync(entries);
            return Ok(result);
        }

        [HttpPost("approve/{transactionId}")]
        public async Task<IActionResult> Approve(int transactionId)
        {
            var result = await _ledgerService.ApproveTransactionAsync(transactionId);
            return Ok(result);
        }

        [HttpPost("reject/{transactionId}")]
        public async Task<IActionResult> Reject(int transactionId)
        {
            var result = await _ledgerService.RejectTransactionAsync(transactionId);
            return Ok(result);
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetByAccount(int accountId)
        {
            var result = await _ledgerService.GetLedgerEntriesAsync(accountId);
            return Ok(result);
        }
    }
}
