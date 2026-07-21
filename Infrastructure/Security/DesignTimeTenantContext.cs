namespace ParkG.Infrastructure.Security;

public sealed class DesignTimeTenantContext : ITenantContext
{
    public Guid? TenantId => null;
    public Guid? OperatorId => null;
    public string? Role => null;
    public bool IsAuthenticated => false;
}