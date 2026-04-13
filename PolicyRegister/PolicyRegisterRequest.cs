using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace InsuranceClaimsApi.PolicyRegister;
    
public class PolicyRegisterRequest : IRequest<PolicyRegisterResponse>
{
    public string PolicyNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string PolicyType { get; set; } = string.Empty;
    public decimal CoverageAmount { get; set; }
    public decimal PremiumAmount { get; set; }

    // ✅ New fields
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PolicyStatus Status { get; set; }
}

public enum PolicyStatus
{
    Active,
    Expired,
    Cancelled,
    Pending
}