using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// JwtOptions lives in the parent HrManagement.Api namespace
using JwtOptions = HrManagement.Api.JwtOptions;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly JwtOptions _jwtOptions;

    public AuthController(IConfiguration configuration)
    {
        _jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<TokenResponse> Login([FromBody] LoginRequest request)
    {
        // Simple hardcoded credentials for demo - replace with proper user store
        if (request.Username != "admin" || request.Password != "changeme")
        {
            return Unauthorized("Invalid credentials");
        }

        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, request.Username),
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, request.Username),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "HR")
        };

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: creds);

        return Ok(new TokenResponse(
            new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token),
            DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes)));
    }
}

public sealed record LoginRequest(string Username, string Password);
public sealed record TokenResponse(string Token, DateTime ExpiresAt);