using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ParkG.Infrastructure.Security;

public class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? TenantId
        => TryGetGuid("tenant_id");

    public Guid? OperatorId
        => TryGetGuid(ClaimTypes.NameIdentifier) ?? TryGetGuid("sub");

    public string? Role
        => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)
           ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("role");

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    private Guid? TryGetGuid(string claimType)
    {
        var claimValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);
        return Guid.TryParse(claimValue, out var parsed) ? parsed : null;
    }
}