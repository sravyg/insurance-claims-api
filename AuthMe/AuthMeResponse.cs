using InsuranceClaimsApi.Models.Helpers;

namespace InsuranceClaimsApi.AuthMe;

public class AuthMeResponse : BaseResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; }
}
