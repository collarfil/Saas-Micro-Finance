using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Saas_Micro_Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;
        private readonly AppTenantInfo? _tenantInfo;
        private readonly IServiceProvider _serviceProvider;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config, IServiceProvider serviceProvider,
            ILogger<AuthController> logger,
            IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _logger = logger;
            _tenantInfo = tenantAccessor.MultiTenantContext?.TenantInfo;
            _serviceProvider = serviceProvider;
        }

        [HttpPost("register-super-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterSuperAdmin([FromBody] RegisterSuperAdminDto dto)
        {
            if (_userManager == null)
            {
                return StatusCode(500, "User manager service is not initialized. Check Program.cs registrations.");
            }

            try
            {
                // Check if Super Admin already exists
                var existingSuperAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
                if (existingSuperAdmins.Any())
                {
                    return BadRequest(new { success = false, message = "Super Admin already exists." });
                }

                // Create roles
                var roles = new[] { "SuperAdmin", "TenantAdmin", "BranchManager", "LoanOfficer", "Teller", "Customer" };
                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Create Super Admin user
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true,
                    //BusinessTenantId = null
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new { success = false, errors = result.Errors.Select(e => e.Description) });
                }

                await _userManager.AddToRoleAsync(user, "SuperAdmin");
                return Ok(new { success = true, message = "Super Admin created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            ApplicationUser? user = null;
            IList<string> roles = new List<string>();

            // 1. Identify the Context (Master vs Tenant)
            if (_tenantInfo == null)
            {
                // SCOPE: Master Database (SuperAdmin Login)
                using var scope = _serviceProvider.CreateScope();
                var adminDb = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
                var userStore = new Microsoft.AspNetCore.Identity.EntityFrameworkCore.UserStore<ApplicationUser>(adminDb);
                var options = new Microsoft.Extensions.Options.OptionsWrapper<IdentityOptions>(new IdentityOptions());

                using var adminUserManager = new UserManager<ApplicationUser>(
                    userStore, options, new PasswordHasher<ApplicationUser>(),
                    null, null, null, null, null, null);

                user = await adminUserManager.FindByEmailAsync(dto.Email);

                if (user != null && await adminUserManager.CheckPasswordAsync(user, dto.Password))
                {
                    roles = await adminUserManager.GetRolesAsync(user);

                    // Safety Check: Only SuperAdmins can login without a tenant header
                    if (!roles.Contains("SuperAdmin"))
                    {
                        return BadRequest(new { error = "Bank staff must provide the __tenant__ header." });
                    }
                }
            }
            else
            {
                // SCOPE: Tenant Database (Bank Staff Login)
                // The default _userManager is already scoped to the tenant via Program.cs
                user = await _userManager.FindByEmailAsync(dto.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
                {
                    roles = await _userManager.GetRolesAsync(user);
                }
            }

            // 2. Final Result Check
            // If we reached this point and user is still null, the login failed.
            if (user == null)
            {
                return Unauthorized(new { error = "Invalid email or password" });
            }
            if (!user.IsActive)
            {
                return Unauthorized(new
                {
                    error = "Your account has been deactivated. Please contact your administrator."
                });
            }
            // 3. Success: Generate and Return Token
            var identifier = _tenantInfo?.Identifier ?? "master";
            var token = GenerateJwtToken(user, identifier, roles);

            return Ok(new
            {
                success = true,

                token,

                expiresInMinutes = int.Parse(
        _config["JwtSettings:DurationInMinutes"] ?? "60"
    ),

                user = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    Roles = roles
                },

                tenant = _tenantInfo == null
        ? new
        {
            Identifier = "master",
            Name = "Master Administration"
        }
        : new
        {
            _tenantInfo.Identifier,
            _tenantInfo.Name
        }
            });
        }

        private string GenerateJwtToken(ApplicationUser user, string tenantIdentifier, IList<string> roles)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtSettings["Key"] ?? throw new Exception("JWT Key Missing")
                )
            );

            var duration = int.Parse(jwtSettings["DurationInMinutes"] ?? "60");

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim("tenant", tenantIdentifier),
        new Claim("FirstName", user.FirstName ?? ""),
        new Claim("LastName", user.LastName ?? "")
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}