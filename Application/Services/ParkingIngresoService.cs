using Microsoft.EntityFrameworkCore;
using ParkG.Application.DTOs.Parking;
using ParkG.Application.Exceptions;
using ParkG.Domain.Entities;
using ParkG.Infrastructure.Data;
using ParkG.Infrastructure.Security;

namespace ParkG.Application.Services;

public class ParkingIngresoService : IParkingIngresoService
{
    private static readonly HashSet<string> AllowedVehicleTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "carro",
        "camion",
        "trailer"
    };

    private readonly ParkGDbContext _context;
    private readonly ITenantContext _tenantContext;

    public ParkingIngresoService(ParkGDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<ParkingIngresoResponse> RegisterIngresoAsync(ParkingIngresoRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException("No se pudo resolver el tenant actual.");
        var operatorId = _tenantContext.OperatorId ?? throw new UnauthorizedAccessException("No se pudo resolver el operador actual.");

        var tipoVehiculo = request.TipoVehiculo.Trim().ToLowerInvariant();
        if (!AllowedVehicleTypes.Contains(tipoVehiculo))
        {
            throw new ParkingValidationException("El tipo de vehículo no es válido.");
        }

        var placa = request.Placa.Trim().ToUpperInvariant();
        var dniCliente = request.DniCliente.Trim();
        var espacioCodigo = request.EspacioCodigo.Trim().ToUpperInvariant();

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var espacio = await LockEspacioAsync(tenantId, espacioCodigo, cancellationToken);
        if (espacio is null)
        {
            throw new ParkingNotFoundException($"No existe el espacio {espacioCodigo} para el tenant actual.");
        }

        if (!string.Equals(espacio.Estado, "libre", StringComparison.OrdinalIgnoreCase))
        {
            throw new ParkingConflictException($"El espacio {espacioCodigo} ya está ocupado.");
        }

        if (!string.Equals(espacio.TipoVehiculoPermitido.Trim(), tipoVehiculo, StringComparison.OrdinalIgnoreCase))
        {
            throw new ParkingValidationException("El tipo de vehículo no es compatible con el espacio solicitado.");
        }

        var estadiaAbierta = await _context.Estadias
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.TenantId == tenantId && e.Placa == placa && e.Estado == "abierta", cancellationToken);

        if (estadiaAbierta is not null)
        {
            throw new ParkingConflictException("Ya existe una estadía abierta para la placa indicada.");
        }

        var estadia = new Estadia
        {
            TenantId = tenantId,
            Placa = placa,
            TipoVehiculo = tipoVehiculo,
            DniCliente = dniCliente,
            EspacioId = espacio.Id,
            OperadorIngresoId = operatorId,
            FechaIngreso = DateTime.UtcNow,
            Estado = "abierta"
        };

        espacio.Estado = "ocupado";

        _context.Estadias.Add(estadia);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new ParkingIngresoResponse(estadia.Id, estadia.Placa, espacio.Codigo, estadia.FechaIngreso);
    }

    private async Task<EspacioParking?> LockEspacioAsync(Guid tenantId, string espacioCodigo, CancellationToken cancellationToken)
    {
        return await _context.EspaciosParking
            .FirstOrDefaultAsync(e => e.TenantId == tenantId && e.Codigo == espacioCodigo, cancellationToken);
    }
}