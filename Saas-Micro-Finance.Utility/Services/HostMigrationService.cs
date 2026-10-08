using Microsoft.EntityFrameworkCore;
using Saas_Micro_Finance.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Utility.Services
{
    public class HostMigrationService
    {
        private readonly AdminDbContext _db;

        public HostMigrationService(AdminDbContext db)
        {
            _db = db;
        }

        public async Task MigrateAsync()
        {
            await _db.Database.MigrateAsync();
        }
    }
}
