using ParkG.Application.DTOs.Auth;

namespace ParkG.Application.Services;

public interface ITenantProvisioningService
{
    Task<RegisterTenantResponse> RegisterAsync(RegisterTenantRequest request, CancellationToken cancellationToken = default);
}