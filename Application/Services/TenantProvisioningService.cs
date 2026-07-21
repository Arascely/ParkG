using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkG.Application.DTOs.Auth;
using ParkG.Application.Exceptions;
using ParkG.Domain.Entities;
using ParkG.Infrastructure.Data;

namespace ParkG.Application.Services;

public class TenantProvisioningService : ITenantProvisioningService
{
    private static readonly string[] DefaultVehicleTypes = ["carro", "camion", "trailer"];
    private readonly ParkGDbContext _context;
    private readonly PasswordHasher<Operador> _passwordHasher = new();

    public TenantProvisioningService(ParkGDbContext context)
    {
        _context = context;
    }

    public async Task<RegisterTenantResponse> RegisterAsync(RegisterTenantRequest request, CancellationToken cancellationToken = default)
    {
        var ruc = request.Ruc.Trim();
        var nombreComercial = request.NombreComercial.Trim();
        var ownerUsername = request.OwnerUsername.Trim();

        var tenantExists = await _context.Tenants
            .AnyAsync(t => t.Ruc == ruc, cancellationToken);

        if (tenantExists)
        {
            throw new TenantAlreadyExistsException(ruc);
        }

        var tenant = new Tenant
        {
            Ruc = ruc,
            NombreComercial = nombreComercial,
            Estado = "activo",
            CreadoEn = DateTime.UtcNow
        };

        var owner = new Operador
        {
            TenantId = tenant.Id,
            Tenant = tenant,
            Username = ownerUsername,
            Rol = "owner",
            Activo = true
        };

        owner.PasswordHash = _passwordHasher.HashPassword(owner, request.OwnerPassword);

        var defaultTariffs = DefaultVehicleTypes.Select(vehicleType => new TarifaTenant
        {
            TenantId = tenant.Id,
            Tenant = tenant,
            TipoVehiculo = vehicleType,
            TarifaHora = 0m,
            TarifaDia = 0m
        }).ToList();

        var defaultSpaces = DefaultVehicleTypes.Select((vehicleType, index) => new EspacioParking
        {
            TenantId = tenant.Id,
            Tenant = tenant,
            Codigo = $"{vehicleType[..1].ToUpperInvariant()}{index + 1:000}",
            TipoVehiculoPermitido = vehicleType,
            Estado = "libre"
        }).ToList();

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        _context.Tenants.Add(tenant);
        _context.Operadores.Add(owner);
        _context.EspaciosParking.AddRange(defaultSpaces);
        _context.TarifasTenant.AddRange(defaultTariffs);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var responseTariffs = defaultTariffs
            .Select(t => new TarifaBaseResponse(t.TipoVehiculo, t.TarifaHora, t.TarifaDia))
            .ToList();

        return new RegisterTenantResponse(tenant.Id, owner.Id, responseTariffs);
    }
}