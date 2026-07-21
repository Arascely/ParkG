using System.ComponentModel.DataAnnotations;

namespace ParkG.Application.DTOs.Parking;

public sealed class ParkingIngresoRequest
{
    [Required]
    [RegularExpression("^[A-Z0-9]{2,4}-[A-Z0-9]{3,4}$", ErrorMessage = "La placa no tiene el formato peruano esperado.")]
    public string Placa { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string TipoVehiculo { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string EspacioCodigo { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^[0-9]{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
    public string DniCliente { get; set; } = string.Empty;
}