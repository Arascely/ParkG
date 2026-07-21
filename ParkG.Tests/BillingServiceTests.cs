using ParkG.Application.Services;

namespace ParkG.Tests;

public class BillingServiceTests
{
    private readonly BillingService _service = new();

    [Fact]
    public void Calculate_Under24Hours_RoundsUpByHour()
    {
        var ingreso = new DateTime(2026, 07, 20, 8, 0, 0, DateTimeKind.Utc);
        var salida = ingreso.AddMinutes(90);

        var result = _service.Calculate(ingreso, salida, 10m, 50m);

        Assert.Equal(90, result.MinutosTotales);
        Assert.Equal(20m, result.TotalBruto);
        Assert.Equal(16.95m, result.SubtotalNeto);
        Assert.Equal(3.05m, result.Igv);
    }

    [Fact]
    public void Calculate_Over24Hours_UsesDaysAndRemainingHours()
    {
        var ingreso = new DateTime(2026, 07, 18, 8, 0, 0, DateTimeKind.Utc);
        var salida = ingreso.AddHours(27);

        var result = _service.Calculate(ingreso, salida, 10m, 50m);

        Assert.Equal(1620, result.MinutosTotales);
        Assert.Equal(80m, result.TotalBruto);
        Assert.Equal(67.80m, result.SubtotalNeto);
        Assert.Equal(12.20m, result.Igv);
    }

    [Fact]
    public void Calculate_RoundsDecimalAmountsToTwoPlaces()
    {
        var ingreso = new DateTime(2026, 07, 20, 8, 0, 0, DateTimeKind.Utc);
        var salida = ingreso.AddMinutes(61);

        var result = _service.Calculate(ingreso, salida, 12.345m, 99.999m);

        Assert.Equal(61, result.MinutosTotales);
        Assert.Equal(24.69m, result.TotalBruto);
        Assert.Equal(20.92m, result.SubtotalNeto);
        Assert.Equal(3.77m, result.Igv);
    }
}