namespace ParkG.Application.Exceptions;

public sealed class ParkingNotFoundException : ParkingException
{
    public ParkingNotFoundException(string message) : base(message)
    {
    }
}