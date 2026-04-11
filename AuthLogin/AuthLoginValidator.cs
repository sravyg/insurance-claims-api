using FluentValidation;

namespace InsuranceClaimsApi.AuthLogin;

public class AuthLoginValidator : AbstractValidator<AuthLoginRequest>
{
    public AuthLoginValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.");
    }
}
