using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkG.Domain.Entities;

namespace ParkG.Infrastructure.Security;

public interface IJwtTokenService
{
    JwtTokenResult GenerateToken(Tenant tenant, Operador operador);
}

public sealed record JwtTokenResult(string AccessToken, int ExpiresIn, DateTimeOffset ExpiresAt);

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public JwtTokenResult GenerateToken(Tenant tenant, Operador operador)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpiryMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, operador.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, operador.Username),
            new("tenant_id", tenant.Id.ToString()),
            new(ClaimTypes.Role, operador.Rol),
            new("role", operador.Rol),
            new("tenant_ruc", tenant.Ruc)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        return new JwtTokenResult(tokenHandler.WriteToken(token), _options.ExpiryMinutes, expiresAt);
    }
}