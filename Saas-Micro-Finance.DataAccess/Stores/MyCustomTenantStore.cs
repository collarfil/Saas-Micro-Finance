using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.Models;

namespace Saas_Micro_Finance.DataAccess.Stores
{
    public class MyCustomTenantStore : IMultiTenantStore<AppTenantInfo>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MyCustomTenantStore(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // 1. Get a specific tenant by the ID (Primary Key)
        public async Task<AppTenantInfo?> TryGetAsync(string id)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
            return await db.TenantInfo.FindAsync(id);
        }

        // 2. Get a specific tenant by its identifier (e.g., "unionbank")
        public async Task<AppTenantInfo?> TryGetByIdentifierAsync(string identifier)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
            return await db.TenantInfo.FirstOrDefaultAsync(t => t.Identifier == identifier);
        }

        // 3. The missing piece: Get all tenants
        public async Task<IEnumerable<AppTenantInfo>> GetAllAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
            return await db.TenantInfo.ToListAsync();
        }

        // Management methods (Can throw NotImplemented if you handle creation in your Controller)
        public Task<bool> TryAddAsync(AppTenantInfo tenantInfo) => throw new NotImplementedException();
        public Task<bool> TryRemoveAsync(string identifier) => throw new NotImplementedException();
        public Task<bool> TryUpdateAsync(AppTenantInfo tenantInfo) => throw new NotImplementedException();
    }
}