namespace ParkG.Application.DTOs.Tarifas;

public sealed record TarifaVehiculoDto(
    string TipoVehiculo,
    decimal TarifaHora,
    decimal TarifaDia);