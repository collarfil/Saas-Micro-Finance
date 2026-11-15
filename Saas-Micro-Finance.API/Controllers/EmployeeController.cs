using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using System.Linq;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeeController(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            var result = new List<object>();

            foreach (var emp in employees)
            {
                string? role = null;
                string? email = null;

                if (!string.IsNullOrEmpty(emp.ApplicationUserId))
                {
                    var user = await _userManager.FindByIdAsync(emp.ApplicationUserId);
                    if (user != null)
                    {
                        email = user.Email;
                        var roles = await _userManager.GetRolesAsync(user);
                        role = roles.FirstOrDefault();
                    }
                }

                result.Add(new
                {
                    emp.Id,
                    emp.ApplicationUserId,
                    emp.FirstName,
                    emp.LastName,
                    emp.Gender,
                    emp.DOB,
                    emp.StaffNumber,
                    emp.DepartmentId,
                    emp.Position,
                    emp.BranchId,
                    emp.Street,
                    Email = email,
                    Role = role
                });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var emp = await _unitOfWork.Employees.GetByIdAsync(id);
            if (emp == null) return NotFound();

            string? role = null;
            string? email = null;

            if (!string.IsNullOrEmpty(emp.ApplicationUserId))
            {
                var user = await _userManager.FindByIdAsync(emp.ApplicationUserId);
                if (user != null)
                {
                    email = user.Email;
                    var roles = await _userManager.GetRolesAsync(user);
                    role = roles.FirstOrDefault();
                }
            }

            return Ok(new
            {
                emp.Id,
                emp.ApplicationUserId,
                emp.FirstName,
                emp.LastName,
                emp.Gender,
                emp.DOB,
                emp.StaffNumber,
                emp.DepartmentId,
                emp.Position,
                emp.BranchId,
                emp.Street,
                Email = email,
                Role = role
            });
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(EmployeeDto dto)
        {
            if (dto.Id == 0)
            {
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);

                if (existingUser != null)
                    return BadRequest("Email already exists.");

                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                if (!await _roleManager.RoleExistsAsync(dto.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(dto.Role));
                }

                await _userManager.AddToRoleAsync(user, dto.Role);

                var employee = new Employee
                {
                    ApplicationUserId = user.Id,

                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Gender = dto.Gender,
                    DOB = dto.DOB,
                    StaffNumber = dto.StaffNumber,
                    DepartmentId = dto.DepartmentId,
                    Position = dto.Position
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