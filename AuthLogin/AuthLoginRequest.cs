using MediatR;

namespace InsuranceClaimsApi.AuthLogin;

public class AuthLoginRequest : IRequest<AuthLoginResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
