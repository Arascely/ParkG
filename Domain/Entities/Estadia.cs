using System;

namespace ParkG.Domain.Entities;

public class Estadia 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string TipoVehiculo { get; set; } = string.Empty;
    public string DniCliente { get; set; } = string.Empty;
    public Guid EspacioId { get; set; }
    public Guid OperadorIngresoId { get; set; }
    public Guid? OperadorSalidaId { get; set; }
    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    public DateTime? FechaSalida { get; set; }
    public string Estado { get; set; } = "abierta";
}