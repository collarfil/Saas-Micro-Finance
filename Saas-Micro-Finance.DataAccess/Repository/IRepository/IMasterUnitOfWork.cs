using Microsoft.EntityFrameworkCore.Storage;
using Saas_Micro_Finance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.DataAccess.Repository.IRepository
{
    public interface IMasterUnitOfWork : IDisposable
    {
        IRepository<AppTenantInfo> AppTenants { get; }
        Task<int> SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
