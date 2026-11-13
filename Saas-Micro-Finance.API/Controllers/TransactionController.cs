using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models.DTOs;
using Saas_Micro_Finance.Utility.Services.Interface;

namespace Saas_Micro_Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IUnitOfWork _unitOfWork;

        public TransactionController(ITransactionService transactionService, IUnitOfWork unitOfWork)
        {
            _transactionService = transactionService;
            _unitOfWork= unitOfWork;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(TransactionDto dto)
        {
            var result = await _transactionService.DepositAsync(
                
                dto.AccountId,
                dto.Amount,
                dto.Reference,
                dto.Narration,
                dto.Channel);

            return Ok(result);
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetAccountTransactions(int accountId)
        {
            // Uses your generic repository filter!
            var txs = await _unitOfWork.Transactions.GetAllAsync(t => t.AccountId == accountId);
            return Ok(txs.OrderByDescending(t => t.Created_At));
        }
    }
}
