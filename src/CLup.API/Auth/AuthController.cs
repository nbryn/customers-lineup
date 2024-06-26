using CLup.API.Auth.Contracts;
using CLup.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProblemDetails = CLup.Application.Shared.ProblemDetails;

namespace CLup.API.Auth;

[ApiController]
[Route("api/")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [Route("register")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
    [ProducesResponseType((typeof(ProblemDetails)), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _mediator.Send(request.MapToCommand());

        return this.CreateActionResult(result, token => new TokenResponse(token));
    }

    [AllowAnonymous]
    [Route("login")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
    [ProducesResponseType((typeof(ProblemDetails)), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType((typeof(ProblemDetails)), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(request.MapToCommand());

        return this.CreateActionResult(result, token => new TokenResponse(token));
    }
}
