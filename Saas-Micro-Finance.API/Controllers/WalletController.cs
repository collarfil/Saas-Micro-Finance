using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var wallets = await _unitOfWork.Wallets.GetAllAsync();
            return Ok(wallets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var wallet = await _unitOfWork.Wallets.GetByIdAsync(id);
            if (wallet == null) return NotFound();
            return Ok(wallet);
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(WalletDto dto)
        {
            Wallet wallet;
            if (dto.Id == 0)
            {
                wallet = new Wallet
                {
                    OwnerType = dto.OwnerType,
                    OwnerId = dto.OwnerId,
                    Balance = dto.Balance
                };
                await _unitOfWork.Wallets.AddAsync(wallet);
            }
            else
            {
                wallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.Id == dto.Id);
                if (wallet == null) return NotFound();

                wallet.OwnerType = dto.OwnerType;
                wallet.OwnerId = dto.OwnerId;
                wallet.Balance = dto.Balance;
                _unitOfWork.Wallets.Update(wallet);
            }
            await _unitOfWork.SaveAsync();
            return Ok(wallet);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var wallet = await _unitOfWork.Wallets.GetByIdAsync(id);
            if (wallet == null) return NotFound();

            _unitOfWork.Wallets.Remove(wallet);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}