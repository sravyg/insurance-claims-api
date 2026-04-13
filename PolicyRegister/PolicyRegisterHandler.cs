using FluentValidation;
using MediatR;
using InsuranceClaimsApi.Services;

namespace InsuranceClaimsApi.PolicyRegister;

public class PolicyRegisterHandler : IRequestHandler<PolicyRegisterRequest, PolicyRegisterResponse>
{
    private readonly IValidator<PolicyRegisterRequest> _validator;
    private readonly IPolicyEventPublisher _policyEventPublisher;

    public PolicyRegisterHandler(
        IValidator<PolicyRegisterRequest> validator,
        IPolicyEventPublisher policyEventPublisher)
    {
        _validator = validator;
        _policyEventPublisher = policyEventPublisher;
    }

    public async Task<PolicyRegisterResponse> Handle(
        PolicyRegisterRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await _policyEventPublisher.PublishPolicyCreatedAsync(request, cancellationToken);

        return new PolicyRegisterResponse
        {
            PolicyNumber = request.PolicyNumber,
            Message = "Policy request validated successfully and message published to AWS SQS."
        };
    }
}