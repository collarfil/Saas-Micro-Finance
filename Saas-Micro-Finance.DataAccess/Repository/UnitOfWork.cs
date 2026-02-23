using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Storage;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.Models;

namespace Saas_Micro_Finance.DataAccess.Repository
{


    public class UnitOfWork : IUnitOfWork
    {
        private readonly SaasBankDbContext _db;
        private IDbContextTransaction? _transaction;

        public IRepository<Account> Accounts { get; private set; }
        public IRepository<Account_Type> AccountTypes { get; private set; }
        public IRepository<Tenant> Tenants { get; private set; }
        public IRepository<Customer> Customers { get; private set; }
        public IRepository<Employee> Employees { get; private set; }
        public IRepository<Notification> Notifications { get; private set; }
        public IRepository<WhatsApp> WhatsApps { get; private set; }
        public IRepository<Loan> Loans { get; private set; }
        public IRepository<LoanProduct> LoanProducts { get; private set; }
        public IRepository<LoanRepayment> LoanRepayments { get; private set; }
        public IRepository<Wallet> Wallets { get; private set; }
        public IRepository<Transaction> Transactions { get; private set; }
        public IRepository<Customer_Address> CustomerAddresses { get; private set; }
        public IRepository<Customer_KYC> CustomerKYCs { get; private set; }
        public IRepository<LedgerEntry> LedgerEntries { get; private set; }
        public IRepository<Branch> Branches { get; private set; }
        public IRepository<AuditLog> AuditLogs { get; private set; }

        public UnitOfWork(SaasBankDbContext db)
        {
            _db = db;

            Accounts = new Repository<Account>(_db);
            AccountTypes = new Repository<Account_Type>(_db);
            Tenants = new Repository<Tenant>(_db);
            Customers = new Repository<Customer>(_db);
            Employees = new Repository<Employee>(_db);
            Notifications = new Repository<Notification>(_db);
            WhatsApps = new Repository<WhatsApp>(_db);
            Loans = new Repository<Loan>(_db);
            LoanProducts = new Repository<LoanProduct>(_db);
            LoanRepayments = new Repository<LoanRepayment>(_db);
            Wallets = new Repository<Wallet>(_db);
            Transactions = new Repository<Transaction>(_db);
            CustomerAddresses = new Repository<Customer_Address>(_db);
            CustomerKYCs = new Repository<Customer_KYC>(_db);
            LedgerEntries = new Repository<LedgerEntry>(_db);
            Branches = new Repository<Branch>(_db);
            AuditLogs = new Repository<AuditLog>(_db);
        }

        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}


