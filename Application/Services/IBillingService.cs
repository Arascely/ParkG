using ParkG.Application.DTOs.Parking;

namespace ParkG.Application.Services;

public interface IBillingService
{
    ParkingLiquidationResult Calculate(DateTime ingresoUtc, DateTime salidaUtc, decimal tarifaHora, decimal tarifaDia);
}