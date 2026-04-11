using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceClaimsApi.AuthLogin;

[ApiController]
[Route("api/v1/auth:login")]
public class AuthLoginController : Controller
{
    private readonly IMediator _mediator;

    public AuthLoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Login(AuthLoginRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}
