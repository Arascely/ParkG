using System;

namespace ParkG.Domain.Entities;

public class Tenant 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Ruc { get; set; } = string.Empty;
    public string NombreComercial { get; set; } = string.Empty;
    public string Estado { get; set; } = "activo";
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
}