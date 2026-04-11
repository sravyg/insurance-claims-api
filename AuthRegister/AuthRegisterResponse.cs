using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsuranceClaimsApi.Models.Helpers;

namespace InsuranceClaimsApi.AuthRegister;

public class AuthRegisterResponse : BaseResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime? TokenExpiresAtUtc { get; set; }
}
