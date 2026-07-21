namespace ParkG.Application.DTOs.Auth;

public sealed record TarifaBaseResponse(string TipoVehiculo, decimal TarifaHora, decimal TarifaDia);

public sealed record RegisterTenantResponse(
    Guid TenantId,
    Guid OwnerOperatorId,
    IReadOnlyCollection<TarifaBaseResponse> DefaultTariffs);