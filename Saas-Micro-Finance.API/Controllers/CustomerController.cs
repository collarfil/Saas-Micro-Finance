using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using System.Security.Claims;
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
            var customerDtos = customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                Gender = c.Gender,
                DOB = c.DOB,
                Status = c.Status,
                Created_At = c.Created_At
            }).ToList();
            return Ok(customerDtos);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email)
                     ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "No email claim in token." });

            var customer = await _unitOfWork.Customers
                .GetFirstOrDefaultAsync(c => c.Email == email);

            if (customer == null)
                return NotFound(new { message = "Customer profile not found in this tenant." });

            return Ok(customer);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _unitOfWork.Customers.GetFirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            var customerDto = new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Gender = customer.Gender,
                DOB = customer.DOB,
                Status = customer.Status,
                Created_At = customer.Created_At
            };
            return Ok(customerDto);
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

                if (string.IsNullOrWhiteSpace(dto.Password))
                {
                    return BadRequest(new
                    {
                        message = "Password is required when creating a new customer."
                    });
                }

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
      
        [HttpPost("{id}/create-login")]
        public async Task<IActionResult> CreateLogin(
        int id,[FromBody] CreateCustomerLoginDto dto)
        {
            var customer = await _unitOfWork.Customers
                .GetFirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound(new
                {
                    message = "Customer not found."
                });

            if (!string.IsNullOrEmpty(customer.ApplicationUserId))
            {
                return BadRequest(new
                {
                    message = "This customer already has a login account."
                });
            }

            var existingUser = await _userManager.FindByEmailAsync(customer.Email);

            if (existingUser != null)
            {
                customer.ApplicationUserId = existingUser.Id;

                _unitOfWork.Customers.Update(customer);
                await _unitOfWork.SaveAsync();

                return Ok(new
                {
                    message = "Existing user account linked to customer."
                });
            }

            var user = new ApplicationUser
            {
                UserName = customer.Email,
                Email = customer.Email,
                PhoneNumber = customer.Phone,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Unable to create login account.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole("Customer"));
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            customer.ApplicationUserId = user.Id;

            _unitOfWork.Customers.Update(customer);

            await _unitOfWork.SaveAsync();

            return Ok(new
            {
                message = "Customer login created successfully.",
                userId = user.Id
            });
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