using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using InsuranceClaimsApi.PolicyRegister;

namespace InsuranceClaimsApi.Services;

// public interface IPolicyEventPublisher
// {
//     Task PublishPolicyCreatedAsync(
//         int policyId,
//         string policyNumber,
//         string customerName,
//         string policyType,
//         string status,
//         CancellationToken cancellationToken = default);
// }
public interface IPolicyEventPublisher
{
    Task PublishPolicyCreatedAsync(
        PolicyRegisterRequest request,
        CancellationToken cancellationToken = default);
}

public class PolicyEventPublisher : IPolicyEventPublisher
{
    private readonly IAmazonSQS _sqs;
    private readonly IConfiguration _configuration;

    public PolicyEventPublisher(IAmazonSQS sqs, IConfiguration configuration)
    {
        _sqs = sqs;
        _configuration = configuration;
    }

    public async Task PublishPolicyCreatedAsync(
        PolicyRegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var queueUrl = _configuration["Sqs:PolicyCreatedQueueUrl"]
            ?? throw new InvalidOperationException("Sqs:PolicyCreatedQueueUrl is missing from configuration.");

        var payload = new
        {
            EventType = "PolicyCreated",
            PolicyNumber = request.PolicyNumber,
            CustomerName = request.CustomerName,
            PolicyType = request.PolicyType,
            CoverageAmount = request.CoverageAmount,
            PremiumAmount = request.PremiumAmount,
            StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc),
            Status = request.Status.ToString(),
            PublishedAtUtc = DateTime.UtcNow
        };

        var messageBody = JsonSerializer.Serialize(payload);

        var sendRequest = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = messageBody,
            MessageGroupId = "policy-created-group",
            MessageDeduplicationId = Guid.NewGuid().ToString()
        };

        await _sqs.SendMessageAsync(sendRequest, cancellationToken);
    }
}



// public class PolicyEventPublisher : IPolicyEventPublisher
// {
//     private readonly IAmazonSQS _sqs;
//     private readonly IConfiguration _configuration;

//     public PolicyEventPublisher(IAmazonSQS sqs, IConfiguration configuration)
//     {
//         _sqs = sqs;
//         _configuration = configuration;
//     }

//     public async Task PublishPolicyCreatedAsync(
//         int policyId,
//         string policyNumber,
//         string customerName,
//         string policyType,
//         string status,
//         CancellationToken cancellationToken = default)
//     {
//         var queueUrl = _configuration["Sqs:PolicyCreatedQueueUrl"]
//             ?? throw new InvalidOperationException("Sqs:PolicyCreatedQueueUrl is missing from configuration.");

//         var payload = new
//         {
//             EventType = "PolicyCreated",
//             PolicyId = policyId,
//             PolicyNumber = policyNumber,
//             CustomerName = customerName,
//             PolicyType = policyType,
//             Status = status,
//             CreatedAtUtc = DateTime.UtcNow
//         };

//         var request = new SendMessageRequest
//         {
//             QueueUrl = queueUrl,
//             MessageBody = JsonSerializer.Serialize(payload),
//             MessageGroupId = "policy-created-group",
//             MessageDeduplicationId = Guid.NewGuid().ToString()
//         };

//         await _sqs.SendMessageAsync(request, cancellationToken);
//     }
// }