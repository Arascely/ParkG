using System;

namespace ParkG.Domain.Entities;

public class EspacioParking 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string TipoVehiculoPermitido { get; set; } = string.Empty;
    public string Estado { get; set; } = "libre";
}