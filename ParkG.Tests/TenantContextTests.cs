using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ParkG.Infrastructure.Security;

namespace ParkG.Tests;

public class TenantContextTests
{
    [Fact]
    public void TenantContext_ResolvesClaimsFromAuthenticatedUser()
    {
        var tenantId = Guid.NewGuid();
        var operatorId = Guid.NewGuid();

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim("tenant_id", tenantId.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, operatorId.ToString()),
                    new Claim(ClaimTypes.Role, "admin")
                ],
                authenticationType: "TestAuth"))
        };

        var tenantContext = new TenantContext(new HttpContextAccessor { HttpContext = httpContext });

        Assert.True(tenantContext.IsAuthenticated);
        Assert.Equal(tenantId, tenantContext.TenantId);
        Assert.Equal(operatorId, tenantContext.OperatorId);
        Assert.Equal("admin", tenantContext.Role);
    }

    [Fact]
    public void TenantContext_ReturnsNullValues_WhenUserIsAnonymous()
    {
        var httpContext = new DefaultHttpContext();
        var tenantContext = new TenantContext(new HttpContextAccessor { HttpContext = httpContext });

        Assert.False(tenantContext.IsAuthenticated);
        Assert.Null(tenantContext.TenantId);
        Assert.Null(tenantContext.OperatorId);
        Assert.Null(tenantContext.Role);
    }
}