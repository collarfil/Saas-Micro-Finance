using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.Models;

public class AdminDbContextFactory : IDesignTimeDbContextFactory<AdminDbContext>
{
    public AdminDbContext CreateDbContext(string[] args)
    {
        // This finds the folder where the .sln usually sits
        string basePath = AppContext.BaseDirectory;

        // Fallback: If we are in PMC, we need to go up to find the Web project
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true) // Check local
            .AddJsonFile("../Saas_Micro_Finance/appsettings.json", optional: true) // Check Web project
            .Build();

        var connectionString = configuration.GetConnectionString("MasterConnection");

        // If both fail, we might be in the wrong directory
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception($"Could not find appsettings.json. Current Dir: {Directory.GetCurrentDirectory()}");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AdminDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AdminDbContext(optionsBuilder.Options);
    }
}