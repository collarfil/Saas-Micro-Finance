using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Saas_Micro_Finance.Models;

namespace Saas_Micro_Finance.DataAccess.Data
{

    /// <summary>
    /// Tenant database - each tenant gets its own instance of this DbContext.
    /// Contains all business tables + Identity tables.
    /// </summary>
    public class SaasBankDbContext : MultiTenantIdentityDbContext<ApplicationUser>
    {
        public SaasBankDbContext(ITenantInfo? tenantInfo, DbContextOptions<SaasBankDbContext> options)
            : base(tenantInfo, options) { }



        public DbSet<Branch> Branches { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Account_Type> Account_Types { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Customer_Address> Customer_Addresses { get; set; }
        public DbSet<Customer_KYC> Customer_KYCs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Paystack> Paystacks { get; set; }
        public DbSet<Flutterwave> Flutterwaves { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LedgerEntry> LedgerEntries { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanProduct> LoanProducts { get; set; }
        public DbSet<LoanRepayment> LoanRepayments { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WhatsApp> WhatsApps { get; set; }

        public DbSet<GLAccount> GLAccounts { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // 1. MUST BE FIRST: Identity + Finbuckle base config
            base.OnModelCreating(builder);

            // 2. Multi-tenancy for business models
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.Namespace != null &&
                    entityType.ClrType.Namespace.Contains("Saas_Micro_Finance.Models"))
                {
                    builder.Entity(entityType.ClrType).IsMultiTenant();
                }
            }

            // 3. Foreign Key Constraints
            builder.Entity<LedgerEntry>()
                .HasOne(l => l.Transaction)
                .WithMany()
                .HasForeignKey(l => l.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LedgerEntry>()
                .HasOne(l => l.Account)
                .WithMany()
                .HasForeignKey(l => l.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. ===============================
            // FINANCIAL PRECISION CONFIGURATION
            // ===============================

            // Transactions
            builder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            // Accounts (VERY IMPORTANT)
            builder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            // Ledger Entries
            builder.Entity<LedgerEntry>()
                .Property(l => l.Amount)
                .HasPrecision(18, 2);

            // Loans
            builder.Entity<Loan>()
                .Property(l => l.Principal)
                .HasPrecision(18, 2);

            builder.Entity<Loan>()
                .Property(l => l.Interest)
                .HasPrecision(18, 4);

            // Loan Products (this is your current warning source)
            builder.Entity<LoanProduct>()
                .Property(lp => lp.InterestRate)
                .HasPrecision(18, 4);

            builder.Entity<LoanProduct>()
                .Property(lp => lp.PenaltyRate)
                .HasPrecision(18, 4);

            // Optional: Subscription (if you have it)
            builder.Entity<Subscription>()
                .Property(s => s.Amount)
                .HasPrecision(18, 2);
            // ========================================
            // GENERAL LEDGER / CHART OF ACCOUNTS
            // ========================================

            builder.Entity<GLAccount>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(g => g.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(g => g.Type)
                    .IsRequired();

                entity.Property(g => g.IsActive)
                    .HasDefaultValue(true);

                // Account codes must be unique within each tenant.
                entity.HasIndex(g => g.Code)
                    .IsUnique();

                // Chart of Accounts hierarchy.
                entity.HasOne(g => g.Parent)
                    .WithMany(g => g.Children)
                    .HasForeignKey(g => g.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
                // General Ledger Accounts
                builder.Entity<GLAccount>(entity =>
                {
                    entity.HasKey(g => g.Id);

                    entity.Property(g => g.Name)
                        .IsRequired()
                        .HasMaxLength(150);

                    entity.Property(g => g.Code)
                        .IsRequired()
                        .HasMaxLength(20);

                    entity.Property(g => g.Type)
                        .IsRequired();

                    entity.Property(g => g.IsActive)
                        .HasDefaultValue(true);

                    // Account codes must be unique within this tenant database.
                    entity.HasIndex(g => g.Code)
                        .IsUnique();

                    entity.HasOne(g => g.Parent)
                        .WithMany(g => g.Children)
                        .HasForeignKey(g => g.ParentId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                // Journal Entry
                builder.Entity<JournalEntry>(entity =>
                {
                    entity.HasKey(j => j.Id);

                    entity.Property(j => j.EntryNumber)
                        .IsRequired()
                        .HasMaxLength(50);

                    entity.Property(j => j.Description)
                        .IsRequired()
                        .HasMaxLength(500);

                    entity.Property(j => j.Reference)
                        .HasMaxLength(100);

                    entity.Property(j => j.SourceModule)
                        .IsRequired()
                        .HasMaxLength(50);

                    entity.Property(j => j.CreatedByUserId)
                        .IsRequired()
                        .HasMaxLength(450);

                    entity.Property(j => j.PostedByUserId)
                        .HasMaxLength(450);

                    entity.Property(j => j.Status)
                        .HasConversion<int>();

                    entity.HasIndex(j => j.EntryNumber)
                        .IsUnique();

                    entity.HasMany(j => j.Lines)
                        .WithOne(l => l.JournalEntry)
                        .HasForeignKey(l => l.JournalEntryId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(j => j.Branch)
                        .WithMany()
                        .HasForeignKey(j => j.BranchId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                // Journal Entry Lines
                builder.Entity<JournalEntryLine>(entity =>
                {
                    entity.HasKey(l => l.Id);

                    entity.Property(l => l.Debit)
                        .HasPrecision(18, 2);

                    entity.Property(l => l.Credit)
                        .HasPrecision(18, 2);

                    entity.Property(l => l.Description)
                        .HasMaxLength(250);

                    entity.HasOne(l => l.GLAccount)
                        .WithMany()
                        .HasForeignKey(l => l.GlAccountId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(l => l.Customer)
                        .WithMany()
                        .HasForeignKey(l => l.CustomerId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasIndex(l => l.JournalEntryId);
                    entity.HasIndex(l => l.GlAccountId);
                });
            });
        }
    }
}