using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ParkG.Application.DTOs.Auth;
using ParkG.Domain.Entities;
using ParkG.Infrastructure.Data;

namespace ParkG.Tests;

public class AuthApiIntegrationTests : IClassFixture<ParkingWebApplicationFactory>
{
    private readonly ParkingWebApplicationFactory _factory;

    public AuthApiIntegrationTests(ParkingWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task LoginEndpoint_EmitsJwtWithTenantAndRoleClaims()
    {
        var tenantId = Guid.NewGuid();
        var operatorId = Guid.NewGuid();
        var ruc = "22345678901";
        var username = "owner2";
        var password = "Secret123!";

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ParkGDbContext>();
            await db.Database.EnsureCreatedAsync();

            var tenant = new Tenant
            {
                Id = tenantId,
                Ruc = ruc,
                NombreComercial = "Garaje Dos",
                Estado = "activo",
                CreadoEn = DateTime.UtcNow
            };

            var operador = new Operador
            {
                Id = operatorId,
                TenantId = tenantId,
                Username = username,
                Rol = "owner",
                Activo = true
            };

            operador.PasswordHash = new PasswordHasher<Operador>().HashPassword(operador, password);

            db.Tenants.Add(tenant);
            db.Operadores.Add(operador);
            await db.SaveChangesAsync();
        }

        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Ruc = ruc,
            Username = username,
            Password = password
        });

        response.EnsureSuccessStatusCode();

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login);
        Assert.Equal(tenantId, login!.TenantId);
        Assert.Equal("owner", login.Role);
        Assert.True(login.ExpiresIn > 0);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(login.AccessToken);
        Assert.Equal(operatorId.ToString(), jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(tenantId.ToString(), jwt.Claims.First(c => c.Type == "tenant_id").Value);
        Assert.Equal("owner", jwt.Claims.First(c => c.Type == ClaimTypes.Role || c.Type == "role").Value);
    }
}