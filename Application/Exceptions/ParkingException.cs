namespace ParkG.Application.Exceptions;

public abstract class ParkingException : Exception
{
    protected ParkingException(string message) : base(message)
    {
    }
}