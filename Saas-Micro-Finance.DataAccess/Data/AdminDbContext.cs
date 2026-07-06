using Finbuckle.MultiTenant.Stores;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Saas_Micro_Finance.Models;

namespace Saas_Micro_Finance.DataAccess.Data
{
    // Inherit standard Identity for your Super Admins
    // Implement the Store interface to satisfy Finbuckle's EFCoreStore
    public class AdminDbContext : IdentityDbContext<ApplicationUser>
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options) { }
        // This is where Finbuckle will look for the tenants
        public DbSet<AppTenantInfo> TenantInfo { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppTenantInfo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Identifier).IsUnique();
            });
        }
    }
}