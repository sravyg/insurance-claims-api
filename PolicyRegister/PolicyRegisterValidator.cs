using FluentValidation;

namespace InsuranceClaimsApi.PolicyRegister;

public class PolicyRegisterValidator : AbstractValidator<PolicyRegisterRequest>
{
    public PolicyRegisterValidator()
    {
        RuleFor(x => x.PolicyNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.PolicyType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CoverageAmount).GreaterThan(0);
        RuleFor(x => x.PremiumAmount).GreaterThan(0);
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("StartDate must be before EndDate");
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
        RuleFor(x => x.Status).IsInEnum();
    }
}