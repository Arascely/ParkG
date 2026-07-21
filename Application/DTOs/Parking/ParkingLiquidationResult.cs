namespace ParkG.Application.DTOs.Parking;

public sealed record ParkingLiquidationResult(
    int MinutosTotales,
    decimal TotalBruto,
    decimal SubtotalNeto,
    decimal Igv);