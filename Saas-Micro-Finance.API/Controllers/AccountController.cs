using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using System.Threading.Tasks;
using AccountStatus=Saas_Micro_Finance.Models.AccountStatus;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _unitOfWork.Accounts.GetAllAsync();
            return Ok(accounts);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = await _unitOfWork.Accounts.GetFirstOrDefaultAsync(a => a.Id == id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }
        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(AccountDto dto)
        {
            Account account;
            if (dto.Id == 0)
            {
                account = new Account
                {
                    TenantId = dto.TenantId,
                    CustomerId = dto.CustomerId,
                    Account_TypeId = dto.Account_TypeId,
                    Account_Number = dto.Account_Number,
                    Balance = dto.Balance,
                    Status = AccountStatus.Active,
                    Opened_At = dto.Opened_At
                };
                await _unitOfWork.Accounts.AddAsync(account); // FIX: Add Account, not AccountDto
            }
            else
            {
                account = await _unitOfWork.Accounts.GetFirstOrDefaultAsync(a => a.Id == dto.Id);
                if (account == null)
                {
                    return NotFound();
                }
                account.TenantId = dto.TenantId;
                account.CustomerId = dto.CustomerId;
                account.Account_TypeId = dto.Account_TypeId;
                account.Account_Number = dto.Account_Number;
                account.Balance = dto.Balance;
                account.Status = AccountStatus.Active;
                account.Opened_At = dto.Opened_At;
                _unitOfWork.Accounts.Update(account);
            }
            await _unitOfWork.SaveAsync();
            return Ok(account);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var account = await _unitOfWork.Accounts.GetFirstOrDefaultAsync(a => a.Id == id);
            if (account == null)
            {
                return NotFound();
            }
            _unitOfWork.Accounts.Remove(account);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
