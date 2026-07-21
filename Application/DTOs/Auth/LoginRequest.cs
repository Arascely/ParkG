using System.ComponentModel.DataAnnotations;

namespace ParkG.Application.DTOs.Auth;

public sealed class LoginRequest
{
    [Required]
    [RegularExpression("^[0-9]{11}$", ErrorMessage = "El RUC debe contener exactamente 11 dígitos numéricos.")]
    public string Ruc { get; set; } = string.Empty;

    [Required]
    [StringLength(80, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}