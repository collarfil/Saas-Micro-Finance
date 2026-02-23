using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saas_Micro_Finance.Models;

namespace Saas_Micro_Finance.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Account> Accounts { get; }
        IRepository<Account_Type> AccountTypes { get; }
        IRepository<Tenant> Tenants { get; }
        IRepository<Customer> Customers { get; }
        IRepository<Customer_Address> CustomerAddresses { get; }
        IRepository<Customer_KYC> CustomerKYCs { get; }
        IRepository<Employee> Employees { get; }
        IRepository<Notification> Notifications { get; }
        IRepository<WhatsApp> WhatsApps { get; }
        IRepository<Loan> Loans { get; }
        IRepository<LoanProduct> LoanProducts { get; }
        IRepository<LoanRepayment> LoanRepayments { get; }
        IRepository<Wallet> Wallets { get; }
        IRepository<Transaction> Transactions { get; }
       
        IRepository<LedgerEntry> LedgerEntries { get; }
        IRepository<Branch> Branches { get; }
        IRepository<AuditLog> AuditLogs { get; }

        Task<int> SaveAsync();

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }

}
