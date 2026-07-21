namespace ParkG.Application.DTOs.Tarifas;

public sealed record SaveTarifasRequest(
    IReadOnlyCollection<TarifaVehiculoDto> Tarifas);