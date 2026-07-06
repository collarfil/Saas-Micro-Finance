using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.Models;

public class SaasBankDbContextFactory : IDesignTimeDbContextFactory<SaasBankDbContext>
{
    public SaasBankDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SaasBankDbContext>();

        var dummyConn = "Server=.;Database=TempSaasBankDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;";

        optionsBuilder.UseSqlServer(dummyConn);

        var dummyTenant = new AppTenantInfo
        {
            Id = "design-time",
            Identifier = "design-time",
            Name = "Design Time",
            ConnectionString = dummyConn
        };

        return new SaasBankDbContext(dummyTenant, optionsBuilder.Options);
    }
}