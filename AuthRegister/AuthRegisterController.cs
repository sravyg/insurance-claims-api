using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace InsuranceClaimsApi.AuthRegister;

[ApiController]
[Route("api/v1/auth:register")]
[AllowAnonymous]
public class AuthRegisterController : Controller
{
    private readonly IMediator _mediator;

    public AuthRegisterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Register(AuthRegisterRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}

