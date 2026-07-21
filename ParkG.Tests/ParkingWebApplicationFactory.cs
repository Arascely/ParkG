using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using ParkG.Infrastructure.Data;

namespace ParkG.Tests;

public sealed class ParkingWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        _connection.Open();

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            var testSettings = new Dictionary<string, string?>
            {
                ["DatabaseProvider"] = "Sqlite",
                ["ConnectionStrings:DefaultConnection"] = "DataSource=:memory:"
            };

            configurationBuilder.AddInMemoryCollection(testSettings);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ParkGDbContext>>();
            services.RemoveAll<ParkGDbContext>();

            services.AddDbContext<ParkGDbContext>(options => options.UseSqlite(_connection));
        });
    }

    public override ValueTask DisposeAsync()
    {
        _connection.Dispose();
        return base.DisposeAsync();
    }
}