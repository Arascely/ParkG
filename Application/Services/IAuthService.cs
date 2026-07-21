using ParkG.Application.DTOs.Auth;

namespace ParkG.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}