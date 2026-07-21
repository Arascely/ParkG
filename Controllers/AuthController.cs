using Microsoft.AspNetCore.Mvc;
using ParkG.Application.DTOs.Auth;
using ParkG.Application.Exceptions;
using ParkG.Application.Services;

namespace ParkG.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ITenantProvisioningService _tenantProvisioningService;
    private readonly IAuthService _authService;

    public AuthController(ITenantProvisioningService tenantProvisioningService, IAuthService authService)
    {
        _tenantProvisioningService = tenantProvisioningService;
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterTenantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterTenantResponse>> Register([FromBody] RegisterTenantRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _tenantProvisioningService.RegisterAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (TenantAlreadyExistsException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (OperatorInactiveException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "El operador está inactivo.", detail = ex.Message });
        }
        catch (AuthenticationFailedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}