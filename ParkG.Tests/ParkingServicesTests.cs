using Moq;
using Microsoft.EntityFrameworkCore;
using ParkG.Application.DTOs.Parking;
using ParkG.Application.Services;
using ParkG.Domain.Entities;
using ParkG.Application.Exceptions;

namespace ParkG.Tests;

public class ParkingServicesTests
{
    [Fact]
    public async Task ParkingIngresoService_RegistersIngresoAndOccupiesSpace()
    {
        var tenantId = Guid.NewGuid();
        var operadorId = Guid.NewGuid();
        var contextTenant = new FixedTenantContext(tenantId, operadorId, "operador");
        var (connection, context) = TestDbContextFactory.CreateContext(contextTenant);
        await using var _ = connection;
        await using var __ = context;

        TestDbContextFactory.SeedTenantGraph(
            context,
            new Tenant { Id = tenantId, Ruc = "12345678901", NombreComercial = "Garaje Uno", Estado = "activo", CreadoEn = DateTime.UtcNow },
            new Operador { Id = operadorId, TenantId = tenantId, Username = "operador1", Rol = "operador", Activo = true, PasswordHash = "hash" },
            new EspacioParking { Id = Guid.NewGuid(), TenantId = tenantId, Codigo = "A001", TipoVehiculoPermitido = "carro", Estado = "libre" },
            new TarifaTenant { Id = Guid.NewGuid(), TenantId = tenantId, TipoVehiculo = "carro", TarifaHora = 10m, TarifaDia = 50m, VigenteDesde = DateTime.UtcNow });

        var service = new ParkingIngresoService(context, contextTenant);

        var response = await service.RegisterIngresoAsync(new ParkingIngresoRequest
        {
            Placa = "ABC-123",
            TipoVehiculo = "CARRO",
            EspacioCodigo = "A001",
            DniCliente = "12345678"
        });

        var espacio = await context.EspaciosParking.FirstAsync();

        Assert.Equal("ABC-123", response.Placa);
        Assert.Equal("A001", response.EspacioCodigo);
        Assert.Equal("ocupado", espacio.Estado);
        Assert.Single(context.Estadias);
    }

    [Fact]
    public async Task TenantIsolation_QueryFiltersOnlyReturnCurrentTenantRows()
    {
        var tenantOneId = Guid.NewGuid();
        var tenantTwoId = Guid.NewGuid();
        var contextTenant = new FixedTenantContext(tenantOneId, Guid.NewGuid(), "operador");
        var (connection, context) = TestDbContextFactory.CreateContext(contextTenant);
        await using var _ = connection;
        await using var __ = context;

        context.Tenants.AddRange(
            new Tenant { Id = tenantOneId, Ruc = "12345678901", NombreComercial = "Garaje Uno", Estado = "activo", CreadoEn = DateTime.UtcNow },
            new Tenant { Id = tenantTwoId, Ruc = "12345678902", NombreComercial = "Garaje Dos", Estado = "activo", CreadoEn = DateTime.UtcNow });

        context.EspaciosParking.AddRange(
            new EspacioParking { Id = Guid.NewGuid(), TenantId = tenantOneId, Codigo = "A001", TipoVehiculoPermitido = "carro", Estado = "libre" },
            new EspacioParking { Id = Guid.NewGuid(), TenantId = tenantTwoId, Codigo = "B001", TipoVehiculoPermitido = "carro", Estado = "libre" });

        await context.SaveChangesAsync();

        var visibleSpaces = await context.EspaciosParking.ToListAsync();

        Assert.Single(visibleSpaces);
        Assert.Equal(tenantOneId, visibleSpaces[0].TenantId);
    }

    [Fact]
    public async Task ParkingIngresoService_SecondIngresoOnSameSpaceThrowsConflict()
    {
        var tenantId = Guid.NewGuid();
        var operadorId = Guid.NewGuid();
        var contextTenant = new FixedTenantContext(tenantId, operadorId, "operador");
        var (connection, context) = TestDbContextFactory.CreateContext(contextTenant);
        await using var _ = connection;
        await using var __ = context;

        TestDbContextFactory.SeedTenantGraph(
            context,
            new Tenant { Id = tenantId, Ruc = "12345678901", NombreComercial = "Garaje Uno", Estado = "activo", CreadoEn = DateTime.UtcNow },
            new Operador { Id = operadorId, TenantId = tenantId, Username = "operador1", Rol = "operador", Activo = true, PasswordHash = "hash" },
            new EspacioParking { Id = Guid.NewGuid(), TenantId = tenantId, Codigo = "A002", TipoVehiculoPermitido = "carro", Estado = "libre" },
            new TarifaTenant { Id = Guid.NewGuid(), TenantId = tenantId, TipoVehiculo = "carro", TarifaHora = 10m, TarifaDia = 50m, VigenteDesde = DateTime.UtcNow });

        var service = new ParkingIngresoService(context, contextTenant);

        await service.RegisterIngresoAsync(new ParkingIngresoRequest
        {
            Placa = "ABC-123",
            TipoVehiculo = "carro",
            EspacioCodigo = "A002",
            DniCliente = "12345678"
        });

        await Assert.ThrowsAsync<ParkingConflictException>(() => service.RegisterIngresoAsync(new ParkingIngresoRequest
        {
            Placa = "ABC-123",
            TipoVehiculo = "carro",
            EspacioCodigo = "A002",
            DniCliente = "12345678"
        }));
    }

    [Fact]
    public async Task ParkingSalidaService_ClosesStadiaAndCreatesReceipt()
    {
        var tenantId = Guid.NewGuid();
        var operadorIngresoId = Guid.NewGuid();
        var operadorSalidaId = Guid.NewGuid();
        var contextTenant = new FixedTenantContext(tenantId, operadorIngresoId, "operador");
        var (connection, context) = TestDbContextFactory.CreateContext(contextTenant);
        await using var _ = connection;
        await using var __ = context;

        var tenant = new Tenant { Id = tenantId, Ruc = "12345678901", NombreComercial = "Garaje Uno", Estado = "activo", CreadoEn = DateTime.UtcNow };
        var espacioId = Guid.NewGuid();
        var estadia = new Estadia
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Placa = "ABC-123",
            TipoVehiculo = "carro",
            DniCliente = "12345678",
            EspacioId = espacioId,
            OperadorIngresoId = operadorIngresoId,
            FechaIngreso = DateTime.UtcNow.AddHours(-3),
            Estado = "abierta"
        };

        TestDbContextFactory.SeedTenantGraph(
            context,
            tenant,
            new Operador { Id = operadorIngresoId, TenantId = tenantId, Username = "operador1", Rol = "operador", Activo = true, PasswordHash = "hash" },
            new EspacioParking { Id = espacioId, TenantId = tenantId, Codigo = "A001", TipoVehiculoPermitido = "carro", Estado = "ocupado" },
            new TarifaTenant { Id = Guid.NewGuid(), TenantId = tenantId, TipoVehiculo = "carro", TarifaHora = 10m, TarifaDia = 50m, VigenteDesde = DateTime.UtcNow.AddDays(-1) });
        context.Estadias.Add(estadia);
        context.SaveChanges();

        var billingMock = new Mock<IBillingService>();
        billingMock
            .Setup(s => s.Calculate(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 10m, 50m))
            .Returns(new ParkingLiquidationResult(180, 20m, 16.95m, 3.05m));

        var service = new ParkingSalidaService(context, contextTenant, billingMock.Object);

        var response = await service.RegisterSalidaAsync(new ParkingSalidaRequest { Placa = "ABC-123" });

        var espacio = await context.EspaciosParking.FirstAsync();
        var estadiaActualizada = await context.Estadias.FirstAsync();

        Assert.Equal("ABC-123", response.Placa);
        Assert.Equal("A001", response.EspacioCodigo);
        Assert.Equal("libre", espacio.Estado);
        Assert.Equal("cerrada", estadiaActualizada.Estado);
        Assert.Single(context.Comprobantes);
        billingMock.Verify(s => s.Calculate(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 10m, 50m), Times.Once);
    }
}