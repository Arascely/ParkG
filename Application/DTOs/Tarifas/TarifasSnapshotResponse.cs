namespace ParkG.Application.DTOs.Tarifas;

public sealed record TarifasSnapshotResponse(
    Guid TenantId,
    IReadOnlyCollection<TarifaVehiculoDto> Tarifas);