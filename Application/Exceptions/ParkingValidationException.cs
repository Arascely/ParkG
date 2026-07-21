namespace ParkG.Application.Exceptions;

public sealed class ParkingValidationException : ParkingException
{
    public ParkingValidationException(string message) : base(message)
    {
    }
}