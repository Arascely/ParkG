using ParkG.Application.DTOs.Parking;

namespace ParkG.Application.Services;

public interface IParkingIngresoService
{
    Task<ParkingIngresoResponse> RegisterIngresoAsync(ParkingIngresoRequest request, CancellationToken cancellationToken = default);
}