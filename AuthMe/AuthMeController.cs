using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceClaimsApi.AuthMe;

[ApiController]
[Authorize]
[Route("api/v1/auth:me")]
public class AuthMeController : Controller
{
    private readonly IMediator _mediator;

    public AuthMeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        var response = await _mediator.Send(new AuthMeRequest());
        return Ok(response);
    }
}
