using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public DepartmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();
            return Ok(departments);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(int id)
        {
            var departments = await _unitOfWork.Departments.GetByIdAsync(id);
            return Ok();
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert(DepartmentDto dto)
        {
            if (dto.Id == 0)
            {
                var department = new Department()
                {

                    Name = dto.Name,
                };
                await _unitOfWork.Departments.AddAsync(department);
            }
            else
            {
                var department = await _unitOfWork.Departments.GetFirstOrDefaultAsync(c => c.Id == dto.Id);
                if (department == null)
                {
                    return NotFound();
                }
                _unitOfWork.Departments.Update(department);
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _unitOfWork.Departments.GetFirstOrDefaultAsync(c => c.Id == id);
            if (department == null)
            {
                return NotFound();
            }
            _unitOfWork.Departments.Remove(department);
            await _unitOfWork.SaveAsync();
            return Ok();
        }

    }
}
