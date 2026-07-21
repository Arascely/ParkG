namespace ParkG.Application.Exceptions;

public sealed class TenantAlreadyExistsException : Exception
{
    public TenantAlreadyExistsException(string ruc)
        : base($"Ya existe un tenant registrado con el RUC {ruc}.")
    {
    }
}