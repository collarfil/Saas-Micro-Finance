using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Saas_Micro_Finance.Models;

namespace Saas_Micro_Finance.DataAccess.Data
{
    public class SaasBankDbContext:IdentityDbContext
    {
        public SaasBankDbContext(DbContextOptions<SaasBankDbContext>options):base(options)
        {
            
        }
        public DbSet<Tenant> Tenants { get; set; }
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

    }
    
}
