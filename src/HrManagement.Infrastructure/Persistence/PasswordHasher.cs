using System.Security.Cryptography;

namespace HrManagement.Infrastructure.Persistence;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string passwordHash, string providedPassword);
}

public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string passwordHash, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(providedPassword))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(providedPassword, passwordHash);
    }
}
