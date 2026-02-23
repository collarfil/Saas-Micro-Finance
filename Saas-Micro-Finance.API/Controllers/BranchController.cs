using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public BranchController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var branches = await _unitOfWork.Branches.GetAllAsync();
            return Ok(branches);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var branch = await _unitOfWork.Branches.GetFirstOrDefaultAsync(c => c.Id == id);
            if (branch == null)
            {
                return NotFound();
            }
            return Ok(branch);
        }
        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(BranchDto dto)
        {
            if (dto.Id == 0)
            {
                var branch=new Branch
                                    {
                    TenantId = dto.TenantId,
                    Address = dto.Address
                   
                };
                await _unitOfWork.Branches.AddAsync(branch);
            }
            else
            {
                var branch = await _unitOfWork.Branches.GetFirstOrDefaultAsync(c => c.Id == dto.Id);
                if (branch == null)
                {
                    return NotFound();
                }
                _unitOfWork.Branches.Update(branch);
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var branch = await _unitOfWork.Branches.GetFirstOrDefaultAsync(c => c.Id == id);
            if (branch == null)
            {
                return NotFound();
               
            }
            _unitOfWork.Branches.Remove(branch);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}