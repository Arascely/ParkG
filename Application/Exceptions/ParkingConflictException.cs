namespace ParkG.Application.Exceptions;

public sealed class ParkingConflictException : ParkingException
{
    public ParkingConflictException(string message) : base(message)
    {
    }
}