namespace ParkG.Application.DTOs.Auth;

public sealed record LoginResponse(
    string AccessToken,
    int ExpiresIn,
    Guid TenantId,
    string Role);