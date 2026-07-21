using System.ComponentModel.DataAnnotations;

namespace ParkG.Application.DTOs.Parking;

public sealed class ParkingSalidaRequest
{
    [Required]
    [RegularExpression("^[A-Z0-9]{2,4}-[A-Z0-9]{3,4}$", ErrorMessage = "La placa no tiene el formato peruano esperado.")]
    public string Placa { get; set; } = string.Empty;
}