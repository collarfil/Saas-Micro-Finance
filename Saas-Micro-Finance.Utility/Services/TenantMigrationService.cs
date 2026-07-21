using Microsoft.EntityFrameworkCore;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;

namespace Saas_Micro_Finance.Utility.Services;

public class TenantMigrationService
{
    private readonly IMasterUnitOfWork _masterUow;

    public TenantMigrationService(IMasterUnitOfWork masterUow)
    {
        _masterUow = masterUow;
    }

    public async Task MigrateAllTenantsAsync()
    {
        var tenants = await _masterUow.AppTenants.GetAllAsync();

        foreach (var tenant in tenants)
        {
            var options = new DbContextOptionsBuilder<SaasBankDbContext>()
                .UseSqlServer(tenant.ConnectionString)
                .Options;

            using var context = new SaasBankDbContext(tenant, options);

            await context.Database.MigrateAsync();
        }
    }
}