using System;

namespace ParkG.Domain.Entities;

public class TarifaTenant 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public string TipoVehiculo { get; set; } = string.Empty;
    public decimal TarifaHora { get; set; }
    public decimal TarifaDia { get; set; }
    public DateTime VigenteDesde { get; set; } = DateTime.UtcNow;
    public DateTime? VigenteHasta { get; set; }
}