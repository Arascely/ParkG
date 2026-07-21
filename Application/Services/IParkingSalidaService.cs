using ParkG.Application.DTOs.Parking;

namespace ParkG.Application.Services;

public interface IParkingSalidaService
{
    Task<ParkingSalidaResponse> RegisterSalidaAsync(ParkingSalidaRequest request, CancellationToken cancellationToken = default);
}