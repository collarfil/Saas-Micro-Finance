using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(EmployeeDto dto)
        {
            if (dto.Id == 0)
            {
                var employee = new Employee
                {
                    FirstName = dto.FirstName,
                    LastName=dto.LastName,
                    Gender = dto.Gender,
                    DOB = dto.DOB,
                    StaffNumber = dto.StaffNumber,
                    DepartmentId = dto.DepartmentId,
                    Position = dto.Position,
                   
                };

                await _unitOfWork.Employees.AddAsync(employee);
            }
            else
            {
                var tenant = await _unitOfWork.Employees.GetByIdAsync(dto.Id);

                if (tenant == null)
                    return NotFound();

               
            }

            await _unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.SaveAsync();

            return Ok();
        }
    }
}