using System;

namespace ParkG.Domain.Entities;

public class RolCatalogo
{
    public short Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}