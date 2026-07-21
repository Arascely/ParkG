using Microsoft.EntityFrameworkCore;
using ParkG.Application.DTOs.Parking;
using ParkG.Application.Exceptions;
using ParkG.Domain.Entities;
using ParkG.Infrastructure.Data;
using ParkG.Infrastructure.Security;

namespace ParkG.Application.Services;

public class ParkingSalidaService : IParkingSalidaService
{
    private readonly ParkGDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly IBillingService _billingService;

    public ParkingSalidaService(ParkGDbContext context, ITenantContext tenantContext, IBillingService billingService)
    {
        _context = context;
        _tenantContext = tenantContext;
        _billingService = billingService;
    }

    public async Task<ParkingSalidaResponse> RegisterSalidaAsync(ParkingSalidaRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException("No se pudo resolver el tenant actual.");
        var operatorId = _tenantContext.OperatorId ?? throw new UnauthorizedAccessException("No se pudo resolver el operador actual.");
        var placa = request.Placa.Trim().ToUpperInvariant();

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var estadia = await LockEstadiaOpenAsync(tenantId, placa, cancellationToken);
        if (estadia is null)
        {
            throw new ParkingNotFoundException("No se encontró una estadía abierta para la placa indicada.");
        }

        var espacio = await LockEspacioByIdAsync(tenantId, estadia.EspacioId, cancellationToken);
        if (espacio is null)
        {
            throw new ParkingNotFoundException("No se encontró el espacio asociado a la estadía.");
        }

        var tarifa = await _context.TarifasTenant
            .IgnoreQueryFilters()
            .Where(t => t.TenantId == tenantId && t.TipoVehiculo == estadia.TipoVehiculo)
            .OrderByDescending(t => t.VigenteDesde)
            .FirstOrDefaultAsync(cancellationToken);

        if (tarifa is null)
        {
            throw new ParkingNotFoundException("No existe una tarifa vigente para el tipo de vehículo indicado.");
        }

        var salidaUtc = DateTime.UtcNow;
        var liquidation = _billingService.Calculate(estadia.FechaIngreso, salidaUtc, tarifa.TarifaHora, tarifa.TarifaDia);

        estadia.FechaSalida = salidaUtc;
        estadia.OperadorSalidaId = operatorId;
        estadia.Estado = "cerrada";
        espacio.Estado = "libre";

        var comprobante = new Comprobante
        {
            TenantId = tenantId,
            EstadiaId = estadia.Id,
            MinutosTotales = liquidation.MinutosTotales,
            SubtotalNeto = liquidation.SubtotalNeto,
            Igv = liquidation.Igv,
            Total = liquidation.TotalBruto,
            CreadoEn = salidaUtc
        };

        _context.Comprobantes.Add(comprobante);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new ParkingSalidaResponse(comprobante.Id, estadia.Placa, espacio.Codigo, comprobante.MinutosTotales, comprobante.SubtotalNeto, comprobante.Igv, comprobante.Total);
    }

    private async Task<Estadia?> LockEstadiaOpenAsync(Guid tenantId, string placa, CancellationToken cancellationToken)
    {
        return await _context.Estadias
            .FirstOrDefaultAsync(e => e.TenantId == tenantId && e.Placa == placa && e.Estado == "abierta", cancellationToken);
    }

    private async Task<EspacioParking?> LockEspacioByIdAsync(Guid tenantId, Guid espacioId, CancellationToken cancellationToken)
    {
        return await _context.EspaciosParking
            .FirstOrDefaultAsync(e => e.TenantId == tenantId && e.Id == espacioId, cancellationToken);
    }
}