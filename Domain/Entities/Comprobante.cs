using System;

namespace ParkG.Domain.Entities;

public class Comprobante
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public Guid EstadiaId { get; set; }
    public Estadia? Estadia { get; set; }
    public int MinutosTotales { get; set; }
    public decimal SubtotalNeto { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
}