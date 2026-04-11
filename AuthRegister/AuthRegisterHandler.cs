using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using InsuranceClaimsApi.Models;
using InsuranceClaimsApi.Services;

namespace InsuranceClaimsApi.AuthRegister;

public class AuthRegisterHandler : IRequestHandler<AuthRegisterRequest, AuthRegisterResponse>
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<AuthUser> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IValidator<AuthRegisterRequest> _validator;

    public AuthRegisterHandler(
        AppDbContext dbContext,
        IPasswordHasher<AuthUser> passwordHasher,
        IJwtTokenService jwtTokenService,
        IValidator<AuthRegisterRequest> validator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _validator = validator;
    }

    public async Task<AuthRegisterResponse> Handle(AuthRegisterRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new AuthRegisterResponse
            {
                Success = false,
                Message = string.Join(" ", validationResult.Errors.Select(x => x.ErrorMessage).Distinct())
            };
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var password = request.Password.Trim();
        var displayName = string.IsNullOrWhiteSpace(request.DisplayName)
            ? null
            : request.DisplayName.Trim();

        var existingUser = await _dbContext.AuthUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (existingUser != null)
        {
            return new AuthRegisterResponse
            {
                Success = false,
                Message = "A user with that email already exists."
            };
        }

        var user = new AuthUser
        {
            Email = email,
            DisplayName = displayName,
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        _dbContext.AuthUsers.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var jwt = _jwtTokenService.CreateToken(user);

        return new AuthRegisterResponse
        {
            Success = true,
            Message = "User created successfully.",
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = jwt.Token,
            TokenExpiresAtUtc = jwt.ExpiresAtUtc
        };
    }
}
