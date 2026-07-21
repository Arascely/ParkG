namespace ParkG.Application.Exceptions;

public class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException()
        : base("Credenciales inválidas.")
    {
    }
}