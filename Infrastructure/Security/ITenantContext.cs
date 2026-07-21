namespace ParkG.Infrastructure.Security;

public interface ITenantContext
{
    Guid? TenantId { get; }
    Guid? OperatorId { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}