using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkG.Application.DTOs.Tarifas;
using ParkG.Domain.Entities;
using ParkG.Infrastructure.Data;
using ParkG.Infrastructure.Security;

namespace ParkG.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/tarifas")]
public class TarifasController : ControllerBase
{
    private static readonly string[] AllowedVehicleTypes = ["carro", "camion", "trailer"];

    private readonly ParkGDbContext _context;
    private readonly ITenantContext _tenantContext;

    public TarifasController(ParkGDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    [HttpGet("current")]
    public async Task<ActionResult<TarifasSnapshotResponse>> GetCurrent(CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        if (tenantId is null)
        {
            return Unauthorized(new { message = "No se pudo resolver el tenant actual." });
        }

        var tariffs = await _context.TarifasTenant
            .Where(t => t.TenantId == tenantId.Value)
            .ToListAsync(cancellationToken);

        var latestTariffs = tariffs
            .GroupBy(t => t.TipoVehiculo)
            .Select(g => g
                .OrderByDescending(x => x.VigenteDesde)
                .Select(x => new TarifaVehiculoDto(x.TipoVehiculo, x.TarifaHora, x.TarifaDia))
                .First())
            .ToList();

        foreach (var vehicleType in AllowedVehicleTypes)
        {
            if (latestTariffs.All(t => !string.Equals(t.TipoVehiculo, vehicleType, StringComparison.OrdinalIgnoreCase)))
            {
                latestTariffs.Add(new TarifaVehiculoDto(vehicleType, 0m, 0m));
            }
        }

        var orderedTariffs = AllowedVehicleTypes
            .Select(vehicleType => latestTariffs.First(t => string.Equals(t.TipoVehiculo, vehicleType, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return Ok(new TarifasSnapshotResponse(tenantId.Value, orderedTariffs));
    }

    [HttpPut("current")]
    public async Task<ActionResult<TarifasSnapshotResponse>> SaveCurrent([FromBody] SaveTarifasRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        if (tenantId is null)
        {
            return Unauthorized(new { message = "No se pudo resolver el tenant actual." });
        }

        var normalized = request.Tarifas
            .Select(t => new TarifaVehiculoDto(
                t.TipoVehiculo.Trim().ToLowerInvariant(),
                decimal.Round(t.TarifaHora, 2, MidpointRounding.AwayFromZero),
                decimal.Round(t.TarifaDia, 2, MidpointRounding.AwayFromZero)))
            .ToList();

        if (normalized.Count != AllowedVehicleTypes.Length || normalized.Any(t => !AllowedVehicleTypes.Contains(t.TipoVehiculo)))
        {
            return BadRequest(new { message = "Debes enviar tarifas para carro, camion y trailer." });
        }

        if (normalized.Any(t => t.TarifaHora < 0 || t.TarifaDia < 0))
        {
            return BadRequest(new { message = "Las tarifas no pueden ser negativas." });
        }

        var currentUtc = DateTime.UtcNow;
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var existingTariffs = await _context.TarifasTenant
            .Where(t => t.TenantId == tenantId.Value && t.VigenteHasta == null)
            .ToListAsync(cancellationToken);

        foreach (var existingTariff in existingTariffs)
        {
            existingTariff.VigenteHasta = currentUtc;
        }

        foreach (var tariff in normalized)
        {
            _context.TarifasTenant.Add(new TarifaTenant
            {
                TenantId = tenantId.Value,
                TipoVehiculo = tariff.TipoVehiculo,
                TarifaHora = tariff.TarifaHora,
                TarifaDia = tariff.TarifaDia,
                VigenteDesde = currentUtc,
                VigenteHasta = null
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Ok(new TarifasSnapshotResponse(tenantId.Value, normalized));
    }
}