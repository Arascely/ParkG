namespace ParkG.Application.DTOs.Parking;

public sealed record ParkingEspacioDisponibleDto(
    Guid Id,
    string Codigo,
    string TipoVehiculoPermitido);