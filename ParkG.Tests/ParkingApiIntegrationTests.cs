using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ParkG.Application.DTOs.Auth;
using ParkG.Application.DTOs.Parking;
using ParkG.Domain.Entities;
using ParkG.Infrastructure.Data;

namespace ParkG.Tests;

public class ParkingApiIntegrationTests : IClassFixture<ParkingWebApplicationFactory>
{
    private readonly ParkingWebApplicationFactory _factory;

    public ParkingApiIntegrationTests(ParkingWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_Login_Ingreso_Salida_Flow_WorksEndToEnd()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ParkGDbContext>();
        await db.Database.EnsureCreatedAsync();

        var client = _factory.CreateClient();

        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterTenantRequest
        {
            Ruc = "12345678901",
            NombreComercial = "Garaje Uno",
            OwnerUsername = "owner1",
            OwnerPassword = "Secret123!"
        });

        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Ruc = "12345678901",
            Username = "owner1",
            Password = "Secret123!"
        });

        loginResponse.EnsureSuccessStatusCode();
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", login!.AccessToken);

        using (var scopeForSpace = _factory.Services.CreateScope())
        {
            var dbForSpace = scopeForSpace.ServiceProvider.GetRequiredService<ParkGDbContext>();
            var tenant = await dbForSpace.Tenants.FirstAsync(t => t.Ruc == "12345678901");
            var space = await dbForSpace.EspaciosParking.IgnoreQueryFilters().FirstAsync(e => e.TenantId == tenant.Id && e.TipoVehiculoPermitido == "carro");

            var ingresoResponse = await client.PostAsJsonAsync("/api/parking/ingreso", new ParkingIngresoRequest
            {
                Placa = "ABC-123",
                TipoVehiculo = "carro",
                EspacioCodigo = space.Codigo,
                DniCliente = "12345678"
            });

            ingresoResponse.EnsureSuccessStatusCode();

            var salidaResponse = await client.PostAsJsonAsync("/api/parking/salida", new ParkingSalidaRequest
            {
                Placa = "ABC-123"
            });

            salidaResponse.EnsureSuccessStatusCode();

            var salida = await salidaResponse.Content.ReadFromJsonAsync<ParkingSalidaResponse>();
            Assert.NotNull(salida);
            Assert.Equal("ABC-123", salida!.Placa);
            Assert.Equal(space.Codigo, salida.EspacioCodigo);
        }
    }
}