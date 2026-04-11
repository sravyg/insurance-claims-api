using System.Security.Claims;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InsuranceClaimsApi.Models;
//using InsuranceClaimsApi.Models;

namespace InsuranceClaimsApi.AuthMe;

public class AuthMeHandler : IRequestHandler<AuthMeRequest, AuthMeResponse>
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthMeHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthMeResponse> Handle(AuthMeRequest request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return new AuthMeResponse
            {
                Success = false,
                Message = "User is not authenticated."
            };
        }

        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub");

        if (!int.TryParse(userIdValue, out var userId))
        {
            return new AuthMeResponse
            {
                Success = false,
                Message = "Token does not contain a valid user id."
            };
        }

        var authUser = await _dbContext.AuthUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (authUser == null)
        {
            return new AuthMeResponse
            {
                Success = false,
                Message = "Authenticated user was not found."
            };
        }

        if (!authUser.IsActive)
        {
            return new AuthMeResponse
            {
                Success = false,
                Message = "Authenticated user is inactive."
            };
        }

        return new AuthMeResponse
        {
            Success = true,
            Message = "Token is valid.",
            UserId = authUser.Id,
            Email = authUser.Email,
            DisplayName = authUser.DisplayName,
            IsActive = authUser.IsActive
        };
    }
}
