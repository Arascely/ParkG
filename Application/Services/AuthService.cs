using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkG.Application.DTOs.Auth;
using ParkG.Application.Exceptions;
using ParkG.Domain.Entities;
using ParkG.Infrastructure.Data;
using ParkG.Infrastructure.Security;

namespace ParkG.Application.Services;

public class AuthService : IAuthService
{
    private readonly ParkGDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly PasswordHasher<Operador> _passwordHasher = new();

    public AuthService(ParkGDbContext context, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var ruc = request.Ruc.Trim();
        var username = request.Username.Trim();

        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Ruc == ruc, cancellationToken);

        if (tenant is null || !string.Equals(tenant.Estado, "activo", StringComparison.OrdinalIgnoreCase))
        {
            throw new AuthenticationFailedException();
        }

        var operador = await _context.Operadores
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.TenantId == tenant.Id && o.Username == username, cancellationToken);

        if (operador is null)
        {
            throw new AuthenticationFailedException();
        }

        if (!operador.Activo)
        {
            throw new OperatorInactiveException();
        }

        var verification = _passwordHasher.VerifyHashedPassword(operador, operador.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            throw new AuthenticationFailedException();
        }

        var token = _jwtTokenService.GenerateToken(tenant, operador);

        return new LoginResponse(token.AccessToken, token.ExpiresIn, tenant.Id, operador.Rol);
    }
}