using Microsoft.EntityFrameworkCore;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Utility.Services.Interface;

namespace Saas_Micro_Finance.Utility.Services;

public class TenantMigrationService : ITenantMigrationService
{
    private readonly AdminDbContext _adminDb;

    public TenantMigrationService(AdminDbContext adminDb)
    {
        _adminDb = adminDb;
    }

    public async Task MigrateAsync()
    {
        var tenants = await _adminDb.TenantInfo.ToListAsync();

        foreach (var tenant in tenants)
        {
            var options =
                new DbContextOptionsBuilder<SaasBankDbContext>()
                    .UseSqlServer(
                        tenant.ConnectionString,
                        sql => sql.MigrationsAssembly(
                            typeof(SaasBankDbContext).Assembly.FullName))
                            .Options;

            using var db =
                new SaasBankDbContext(tenant, options);

            await db.Database.MigrateAsync();

            await SeedRoles(db);

            await SeedPermissions(db);

            await SeedDefaultData(db);
        }
    }

    private Task SeedRoles(SaasBankDbContext db)
    {
        return Task.CompletedTask;
    }

    private Task SeedPermissions(SaasBankDbContext db)
    {
        return Task.CompletedTask;
    }

    private Task SeedDefaultData(SaasBankDbContext db)
    {
        return Task.CompletedTask;
    }
}