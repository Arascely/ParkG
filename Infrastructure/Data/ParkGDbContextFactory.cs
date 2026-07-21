using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ParkG.Infrastructure.Security;

namespace ParkG.Infrastructure.Data;

public class ParkGDbContextFactory : IDesignTimeDbContextFactory<ParkGDbContext>
{
    public ParkGDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ParkGDbContext>();
        ParkGDatabaseConfiguration.Configure(optionsBuilder, configuration);

        return new ParkGDbContext(optionsBuilder.Options, new DesignTimeTenantContext());
    }
}