namespace ParkG.Application.Exceptions;

public sealed class OperatorInactiveException : AuthenticationFailedException
{
    public OperatorInactiveException()
        : base()
    {
    }
}