namespace ParkG.Application.DTOs.Parking;

public sealed record ParkingSalidaResponse(
    Guid TicketId,
    string Placa,
    string EspacioCodigo,
    int MinutosTotales,
    decimal SubtotalNeto,
    decimal Igv,
    decimal Total);