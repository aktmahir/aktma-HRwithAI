using System.Security.Claims;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Employees;
using HrManagement.Infrastructure.Metrics;
using HrManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// JwtOptions lives in the parent HrManagement.Api namespace
using JwtOptions = HrManagement.Api.JwtOptions;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher passwordHasher,
    JwtOptions jwtOptions,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Unauthorized("Invalid credentials");
        }

        var user = await users.GetByUsernameAsync(request.Username, cancellationToken);
        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            BusinessMetrics.AuthLoginFailures.Inc();
            return Unauthorized("Invalid credentials");
        }

        var tokenResult = GenerateJwtToken(user);
        BusinessMetrics.AuthLogins.WithLabels(user.Username).Inc();
        var refreshToken = new RefreshToken(
            user.Id,
            Guid.NewGuid().ToString("N"),
            DateTimeOffset.UtcNow.AddDays(jwtOptions.RefreshTokenExpiryDays));
        await refreshTokens.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(new TokenResponse(
            tokenResult.AccessToken,
            tokenResult.ExpiresAt,
            refreshToken.Token,
            jwtOptions.RefreshTokenExpiryDays));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Unauthorized("Invalid refresh token");
        }

        var storedToken = await refreshTokens.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken is null || storedToken.IsExpired)
        {
            return Unauthorized("Invalid or expired refresh token");
        }

        if (storedToken.IsUsed)
        {
            return Unauthorized("Refresh token has already been used");
        }

        if (storedToken.IsRevoked)
        {
            return Unauthorized("Refresh token has been revoked");
        }

        var user = await users.GetByIdAsync(storedToken.UserId, cancellationToken);
        if (user is null)
        {
            return Unauthorized("Invalid refresh token");
        }

        var tokenResult = GenerateJwtToken(user);
        var newRefreshToken = new RefreshToken(
            user.Id,
            Guid.NewGuid().ToString("N"),
            DateTimeOffset.UtcNow.AddDays(jwtOptions.RefreshTokenExpiryDays));
        storedToken.Revoke(newRefreshToken.Token);
        await refreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(new TokenResponse(
            tokenResult.AccessToken,
            tokenResult.ExpiresAt,
            newRefreshToken.Token,
            jwtOptions.RefreshTokenExpiryDays));
    }

    private (string AccessToken, DateTime ExpiresAt) GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var expiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.ExpiryMinutes);
        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        var accessToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        return (accessToken, expiresAt);
    }
}

public sealed record LoginRequest(string Username, string Password);

public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record TokenResponse(string AccessToken, DateTime ExpiresAt, string? RefreshToken = null, int? RefreshTokenExpiresInDays = null);
