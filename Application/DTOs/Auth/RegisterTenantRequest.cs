using System.ComponentModel.DataAnnotations;

namespace ParkG.Application.DTOs.Auth;

public sealed class RegisterTenantRequest
{
    [Required]
    [RegularExpression("^[0-9]{11}$", ErrorMessage = "El RUC debe contener exactamente 11 dígitos numéricos.")]
    public string Ruc { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string NombreComercial { get; set; } = string.Empty;

    [Required]
    [StringLength(80, MinimumLength = 3)]
    public string OwnerUsername { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 6)]
    public string OwnerPassword { get; set; } = string.Empty;
}