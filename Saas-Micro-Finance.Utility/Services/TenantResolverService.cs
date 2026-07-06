using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Utility.Services
{
    public class TenantResolverService
    {
        private readonly AdminDbContext _adminDb;

        public TenantResolverService(AdminDbContext adminDb)
        {
            _adminDb = adminDb;
        }
        private async Task<AppTenantInfo?> DetectTenantFromEmail(string email)
        {
            var domain = email.Split('@').Last().ToLower();

            return await _adminDb.TenantInfo
                .FirstOrDefaultAsync(t =>
                    domain.Contains(t.Identifier.ToLower()));
        }
    }
     
}
