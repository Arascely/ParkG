namespace ParkG.Application.DTOs.Parking;

public sealed record ParkingIngresoResponse(
    Guid TicketId,
    string Placa,
    string EspacioCodigo,
    DateTime FechaIngreso);