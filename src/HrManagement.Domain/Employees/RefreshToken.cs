using HrManagement.Domain.Common;

namespace HrManagement.Domain.Employees;

public sealed class RefreshToken : Entity
{
    private RefreshToken()
    {
    }

    public RefreshToken(Guid userId, string token, DateTimeOffset expiresAt)
    {
        ChangeUserId(userId);
        ChangeToken(token);
        ChangeExpiresAt(expiresAt);
    }

    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public bool IsActive => !IsUsed && !IsRevoked && !IsExpired;

    public void Use()
    {
        if (IsUsed)
        {
            throw new InvalidOperationException("Refresh token has already been used.");
        }

        IsUsed = true;
        MarkUpdated();
    }

    public void Revoke(string? replacedByToken = null)
    {
        if (IsRevoked)
        {
            return;
        }

        IsRevoked = true;
        RevokedAt = DateTimeOffset.UtcNow;
        ReplacedByToken = replacedByToken;
        MarkUpdated();
    }

    private void ChangeUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id is required.", nameof(userId));
        }

        UserId = userId;
    }

    private void ChangeToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token is required.", nameof(token));
        }

        Token = token;
    }

    private void ChangeExpiresAt(DateTimeOffset expiresAt)
    {
        if (expiresAt <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentException("Expires at must be in the future.", nameof(expiresAt));
        }

        ExpiresAt = expiresAt;
    }
}
