using InsuranceClaimsApi.Models.Helpers;

namespace InsuranceClaimsApi.AuthLogin;

public class AuthLoginResponse : BaseResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime? TokenExpiresAtUtc { get; set; }
}
