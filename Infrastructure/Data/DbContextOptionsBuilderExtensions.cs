using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ParkG.Infrastructure.Data;

public static class ParkGDatabaseConfiguration
{
    public static void Configure(
        DbContextOptionsBuilder optionsBuilder,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? "Data Source=ParkGLocal.db";

        optionsBuilder.UseSqlite(connectionString);
    }
}