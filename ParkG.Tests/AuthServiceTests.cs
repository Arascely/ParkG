using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ParkG.Application.DTOs.Auth;
using ParkG.Application.Services;
using ParkG.Domain.Entities;
using ParkG.Infrastructure.Data;
using ParkG.Infrastructure.Security;

namespace ParkG.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ReturnsJwtWithTenantClaim()
    {
        var tenantId = Guid.NewGuid();
        var operadorId = Guid.NewGuid();
        var tenantContext = new FixedTenantContext();
        var (connection, context) = TestDbContextFactory.CreateContext(tenantContext);
        await using var _ = connection;
        await using var __ = context;

        var tenant = new Tenant { Id = tenantId, Ruc = "12345678901", NombreComercial = "Garaje Uno", Estado = "activo", CreadoEn = DateTime.UtcNow };
        var operador = new Operador { Id = operadorId, TenantId = tenantId, Username = "admin", Rol = "owner", Activo = true };
        operador.PasswordHash = new PasswordHasher<Operador>().HashPassword(operador, "Secret123!");

        context.Tenants.Add(tenant);
        context.Operadores.Add(operador);
        await context.SaveChangesAsync();

        var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtOptions
        {
            Issuer = "ParkG",
            Audience = "ParkG",
            SigningKey = "DEV_ONLY_CHANGE_ME_32CHAR_MINIMUM_KEY_123",
            ExpiryMinutes = 120
        });

        var authService = new AuthService(context, new JwtTokenService(jwtOptions));

        var response = await authService.LoginAsync(new LoginRequest
        {
            Ruc = tenant.Ruc,
            Username = "admin",
            Password = "Secret123!"
        });

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(response.AccessToken);

        Assert.Equal(tenantId.ToString(), token.Claims.First(c => c.Type == "tenant_id").Value);
        Assert.Equal("owner", token.Claims.First(c => c.Type == ClaimTypes.Role || c.Type == "role").Value);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("owner", response.Role);
    }
}