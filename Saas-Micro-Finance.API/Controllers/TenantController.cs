using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using System.Data.SqlClient;

namespace Saas_Micro_Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class TenantController : ControllerBase
    {
        private readonly IMasterUnitOfWork _masterUow; // Your repo
        private readonly IServiceProvider _serviceProvider;

        public TenantController(IMasterUnitOfWork masterUow, IServiceProvider serviceProvider)
        {
            _masterUow = masterUow;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetTenants()
        {
            var tenants = await _masterUow.AppTenants.GetAllAsync();
            return Ok(tenants);
        }
        

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetTenantById(string id)
        {
            var tenant = await _masterUow.AppTenants.GetFirstOrDefaultAsync(t => t.Id == id);

            if (tenant == null)
                return NotFound();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] TenantDto dto)
        {
            // 1. Validation (Keep your logic)
            var existing = await _masterUow.AppTenants.GetFirstOrDefaultAsync(t => t.Identifier == dto.Identifier);
            if (existing != null) return BadRequest(new { message = "Tenant identifier already in use." });

            var dbName = $"SaasBank_{dto.Identifier}";
            // Use a variable for Server name for easier deployment later
            var server = ".";
            var connString = $"Server={server};Database={dbName};Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;";

            var tenant = new AppTenantInfo
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = dto.Identifier,
                Name = dto.Name,
                ConnectionString = connString
            };

            // 2. Save to Master
            await _masterUow.AppTenants.AddAsync(tenant);
            await _masterUow.SaveAsync();

            // 3. Create & Migrate Tenant Database
            var optionsBuilder = new DbContextOptionsBuilder<SaasBankDbContext>();
            optionsBuilder.UseSqlServer(connString);

            // We manually instantiate SaasBankDbContext for the new DB
            using (var tenantContext = new SaasBankDbContext(tenant, optionsBuilder.Options))
            {
                await tenantContext.Database.MigrateAsync();

                // 4. SEEDING (Refactored to use the specific context)
                await SeedTenantAdmin(tenantContext, dto.AdminEmail, dto.AdminPassword);
            }

            return Ok(new { message = $"Tenant {dto.Name} created and seeded successfully." });
        }

        private async Task SeedTenantAdmin(SaasBankDbContext context, string email, string password)
        {
            // 1. Create specialized stores for this specific tenant's DB connection
            var userStore = new UserStore<ApplicationUser>(context);
            var roleStore = new RoleStore<IdentityRole>(context);

            // 2. Instantiate Managers manually (bypassing DI)
            var userManager = new UserManager<ApplicationUser>(userStore, null, new PasswordHasher<ApplicationUser>(), null, null, null, null, null, null);
            var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);

            // 3. Create the Role in the Tenant DB
            if (!await roleManager.RoleExistsAsync("TenantAdmin"))
            {
                await roleManager.CreateAsync(new IdentityRole("TenantAdmin"));
            }

            // 4. Create the User in the Tenant DB
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = "Admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "TenantAdmin");
            }
        }
    }
}