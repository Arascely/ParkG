using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkG.Application.DTOs.Parking;
using ParkG.Application.Exceptions;
using ParkG.Application.Services;
using ParkG.Infrastructure.Data;
using ParkG.Infrastructure.Security;

namespace ParkG.Controllers;

[ApiController]
[Authorize]
[Route("api/parking")]
public class ParkingController : ControllerBase
{
    private readonly IParkingIngresoService _parkingIngresoService;
    private readonly IParkingSalidaService _parkingSalidaService;
    private readonly ParkGDbContext _context;
    private readonly ITenantContext _tenantContext;

    public ParkingController(
        IParkingIngresoService parkingIngresoService,
        IParkingSalidaService parkingSalidaService,
        ParkGDbContext context,
        ITenantContext tenantContext)
    {
        _parkingIngresoService = parkingIngresoService;
        _parkingSalidaService = parkingSalidaService;
        _context = context;
        _tenantContext = tenantContext;
    }

    [HttpGet("espacios-libres")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ParkingEspacioDisponibleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<ParkingEspacioDisponibleDto>>> EspaciosLibres([FromQuery] string? tipoVehiculo, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        if (tenantId is null)
        {
            return Unauthorized(new { message = "No se pudo resolver el tenant actual." });
        }

        var query = _context.EspaciosParking
            .AsNoTracking()
            .Where(e => e.TenantId == tenantId.Value && e.Estado == "libre");

        if (!string.IsNullOrWhiteSpace(tipoVehiculo))
        {
            var normalizedVehicle = tipoVehiculo.Trim().ToLowerInvariant();
            query = query.Where(e => e.TipoVehiculoPermitido.ToLower() == normalizedVehicle);
        }

        var spaces = await query
            .OrderBy(e => e.Codigo)
            .Select(e => new ParkingEspacioDisponibleDto(e.Id, e.Codigo, e.TipoVehiculoPermitido))
            .ToListAsync(cancellationToken);

        return Ok(spaces);
    }

    [HttpPost("ingreso")]
    [ProducesResponseType(typeof(ParkingIngresoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ParkingIngresoResponse>> Ingreso([FromBody] ParkingIngresoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _parkingIngresoService.RegisterIngresoAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (ParkingValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ParkingNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ParkingConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("salida")]
    [ProducesResponseType(typeof(ParkingSalidaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ParkingSalidaResponse>> Salida([FromBody] ParkingSalidaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _parkingSalidaService.RegisterSalidaAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (ParkingValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ParkingNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ParkingConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}