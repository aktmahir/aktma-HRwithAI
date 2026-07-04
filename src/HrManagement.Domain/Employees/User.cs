using HrManagement.Domain.Common;

namespace HrManagement.Domain.Employees;

public sealed class User : Entity
{
    private User()
    {
    }

    public User(string username, string passwordHash, string role, Guid? employeeId = null)
    {
        ChangeUsername(username);
        ChangePasswordHash(passwordHash);
        ChangeRole(role);
        LinkEmployee(employeeId);
    }

    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public Guid? EmployeeId { get; private set; }

    public void ChangeUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }

        Username = username.Trim().ToLowerInvariant();
        MarkUpdated();
    }

    public void ChangePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        PasswordHash = passwordHash;
        MarkUpdated();
    }

    public void ChangeRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            throw new ArgumentException("Role is required.", nameof(role));
        }

        Role = role.Trim();
        MarkUpdated();
    }

    public void LinkEmployee(Guid? employeeId)
    {
        EmployeeId = employeeId;
        MarkUpdated();
    }
}
