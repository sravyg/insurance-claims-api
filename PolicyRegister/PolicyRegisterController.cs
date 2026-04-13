using MediatR;
using Microsoft.AspNetCore.Mvc;
using InsuranceClaimsApi.PolicyRegister;
using Microsoft.AspNetCore.Authorization;

namespace InsuranceClaimsApi.Controllers;

[ApiController]
[Authorize]
[Route("api/policy-register")]
public class PolicyRegisterController : ControllerBase
{
    private readonly IMediator _mediator;

    public PolicyRegisterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<PolicyRegisterResponse>> CreatePolicy(
        [FromBody] PolicyRegisterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}