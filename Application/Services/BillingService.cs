using ParkG.Application.DTOs.Parking;

namespace ParkG.Application.Services;

public class BillingService : IBillingService
{
    private const decimal IgvRate = 1.18m;

    public ParkingLiquidationResult Calculate(DateTime ingresoUtc, DateTime salidaUtc, decimal tarifaHora, decimal tarifaDia)
    {
        if (salidaUtc < ingresoUtc)
        {
            throw new ArgumentException("La fecha de salida no puede ser menor que la de ingreso.");
        }

        var totalMinutes = (int)Math.Ceiling((salidaUtc - ingresoUtc).TotalMinutes);
        decimal totalBruto;

        if (totalMinutes < 1440)
        {
            var horas = (int)Math.Ceiling(totalMinutes / 60m);
            totalBruto = horas * tarifaHora;
        }
        else
        {
            var dias = totalMinutes / 1440;
            var minutosRestantes = totalMinutes % 1440;
            var horasRestantes = (int)Math.Ceiling(minutosRestantes / 60m);
            totalBruto = (dias * tarifaDia) + (horasRestantes * tarifaHora);
        }

        totalBruto = decimal.Round(totalBruto, 2, MidpointRounding.AwayFromZero);
        var subtotalNeto = decimal.Round(totalBruto / IgvRate, 2, MidpointRounding.AwayFromZero);
        var igv = decimal.Round(totalBruto - subtotalNeto, 2, MidpointRounding.AwayFromZero);

        return new ParkingLiquidationResult(totalMinutes, totalBruto, subtotalNeto, igv);
    }
}