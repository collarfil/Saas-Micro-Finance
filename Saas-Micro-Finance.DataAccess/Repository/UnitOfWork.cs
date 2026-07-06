// Saas-Micro-Finance.DataAccess/Repository/UnitOfWork.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Finbuckle.MultiTenant;

namespace Saas_Micro_Finance.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SaasBankDbContext _db;
        private readonly AdminDbContext _masterDb;
        private readonly IServiceProvider _serviceProvider;
        private IDbContextTransaction? _transaction;



        // Tenant DB Repositories (all business entities including Tenant)

        public IRepository<Customer> Customers { get; private set; }
        public IRepository<Customer_Address> CustomerAddresses { get; private set; }
        public IRepository<Customer_KYC> CustomerKYCs { get; private set; }
        public IRepository<Account> Accounts { get; private set; }
        public IRepository<Account_Type> AccountTypes { get; private set; }
        public IRepository<Transaction> Transactions { get; private set; }
        public IRepository<LedgerEntry> LedgerEntries { get; private set; }
        public IRepository<Loan> Loans { get; private set; }
        public IRepository<LoanProduct> LoanProducts { get; private set; }
        public IRepository<LoanRepayment> LoanRepayments { get; private set; }
        public IRepository<Notification> Notifications { get; private set; }
        public IRepository<Subscription> Subscriptions { get; private set; }
        public IRepository<Employee> Employees { get; private set; }
        public IRepository<Branch> Branches { get; private set; }
        public IRepository<AuditLog> AuditLogs { get; private set; }
        public IRepository<Paystack> PaystackPayments { get; private set; }
        public IRepository<Flutterwave> FlutterwavePayments { get; private set; }
        public IRepository<Department> Departments { get; private set; }
        public IRepository<Wallet> Wallets { get; private set; }
        public IRepository<WhatsApp> WhatsApps { get; private set; }



        public UnitOfWork(
            SaasBankDbContext db,
            AdminDbContext masterDb,
            IServiceProvider serviceProvider)
        {
            _db = db;
            _masterDb = masterDb;
            _serviceProvider = serviceProvider;



            // Initialize Tenant DB Repositories (using flexible Repository with DbContext)

            Customers = new Repository<Customer>(_db);
            CustomerAddresses = new Repository<Customer_Address>(_db);
            CustomerKYCs = new Repository<Customer_KYC>(_db);
            Accounts = new Repository<Account>(_db);
            AccountTypes = new Repository<Account_Type>(_db);
            Transactions = new Repository<Transaction>(_db);
            LedgerEntries = new Repository<LedgerEntry>(_db);
            Loans = new Repository<Loan>(_db);
            LoanProducts = new Repository<LoanProduct>(_db);
            LoanRepayments = new Repository<LoanRepayment>(_db);
            Notifications = new Repository<Notification>(_db);
            Subscriptions = new Repository<Subscription>(_db);
            Employees = new Repository<Employee>(_db);
            Branches = new Repository<Branch>(_db);
            AuditLogs = new Repository<AuditLog>(_db);
            PaystackPayments = new Repository<Paystack>(_db);
            FlutterwavePayments = new Repository<Flutterwave>(_db);
            Departments = new Repository<Department>(_db);
            Wallets = new Repository<Wallet>(_db);
            WhatsApps = new Repository<WhatsApp>(_db);
        }

        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // Legacy method names for backward compatibility
        public async Task CommitAsync()
        {
            await CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            await RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _db.Dispose();
        }
    }
}