using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InsuranceClaimsApi.Models;
using InsuranceClaimsApi.Services;

namespace InsuranceClaimsApi.AuthLogin;

public class AuthLoginHandler : IRequestHandler<AuthLoginRequest, AuthLoginResponse>
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<AuthUser> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IValidator<AuthLoginRequest> _validator;

    public AuthLoginHandler(
        AppDbContext dbContext,
        IPasswordHasher<AuthUser> passwordHasher,
        IJwtTokenService jwtTokenService,
        IValidator<AuthLoginRequest> validator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _validator = validator;
    }

    public async Task<AuthLoginResponse> Handle(AuthLoginRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new AuthLoginResponse
            {
                Success = false,
                Message = string.Join(" ", validationResult.Errors.Select(x => x.ErrorMessage).Distinct())
            };
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var password = request.Password.Trim();

        var user = await _dbContext.AuthUsers
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (user == null)
        {
            return new AuthLoginResponse
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        if (!user.IsActive)
        {
            return new AuthLoginResponse
            {
                Success = false,
                Message = "User is inactive."
            };
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return new AuthLoginResponse
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        var jwt = _jwtTokenService.CreateToken(user);

        return new AuthLoginResponse
        {
            Success = true,
            Message = "Login successful.",
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = jwt.Token,
            TokenExpiresAtUtc = jwt.ExpiresAtUtc
        };
    }
}
