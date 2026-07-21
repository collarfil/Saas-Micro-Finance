using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using System.Threading.Tasks;
using CustomerStatus = Saas_Micro_Finance.Models.CustomerStatus;

namespace Saas_Micro_Finance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CustomerController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            return Ok(customers);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _unitOfWork.Customers.GetFirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert(CustomerDto dto)
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
                    PhoneNumber = dto.Phone,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                if (!await _roleManager.RoleExistsAsync("Customer"))
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));

                await _userManager.AddToRoleAsync(user, "Customer");

                var customer = new Customer
                {
                    ApplicationUserId = user.Id,

                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Gender = dto.Gender,
                    DOB = dto.DOB,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Created_At = DateTime.UtcNow,
                    Status = CustomerStatus.Active
                };

                await _unitOfWork.Customers.AddAsync(customer);
            }
            else
            {
                var customer = await _unitOfWork.Customers.GetFirstOrDefaultAsync(c => c.Id == dto.Id);
                if (customer == null)
                    return NotFound();

                customer.FirstName = dto.FirstName;
                customer.LastName = dto.LastName;
                customer.Gender = dto.Gender;
                customer.DOB = dto.DOB;
                customer.Email = dto.Email;
                customer.Phone = dto.Phone;
                customer.Status = dto.Status;

                _unitOfWork.Customers.Update(customer);
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _unitOfWork.Customers.GetFirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            _unitOfWork.Customers.Remove(customer);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}