using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ParkG.Domain.Entities;
using ParkG.Infrastructure.Data;
using ParkG.Infrastructure.Security;

namespace ParkG.Tests;

public sealed class FixedTenantContext : ITenantContext
{
    public FixedTenantContext(Guid? tenantId = null, Guid? operatorId = null, string? role = null)
    {
        TenantId = tenantId;
        OperatorId = operatorId;
        Role = role;
    }

    public Guid? TenantId { get; }
    public Guid? OperatorId { get; }
    public string? Role { get; }
    public bool IsAuthenticated => TenantId.HasValue;
}

public static class TestDbContextFactory
{
    public static (SqliteConnection Connection, ParkGDbContext Context) CreateContext(FixedTenantContext tenantContext)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ParkGDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ParkGDbContext(options, tenantContext);
        context.Database.EnsureCreated();

        return (connection, context);
    }

    public static void SeedTenantGraph(ParkGDbContext context, Tenant tenant, Operador operador, EspacioParking espacio, TarifaTenant tarifa)
    {
        context.Tenants.Add(tenant);
        context.Operadores.Add(operador);
        context.EspaciosParking.Add(espacio);
        context.TarifasTenant.Add(tarifa);
        context.SaveChanges();
    }
}