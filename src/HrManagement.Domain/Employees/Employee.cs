using HrManagement.Domain.Common;

namespace HrManagement.Domain.Employees;

public sealed class Employee : Entity
{
    private Employee()
    {
    }

    public Employee(string firstName, string lastName, string email, Guid departmentId, Guid roleId)
    {
        ChangeName(firstName, lastName);
        ChangeEmail(email);
        AssignDepartment(departmentId);
        AssignRole(roleId);
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public Guid DepartmentId { get; private set; }
    public Department? Department { get; private set; }
    public Guid RoleId { get; private set; }
    public Role? Role { get; private set; }

    public void ChangeName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MarkUpdated();
    }

    public void ChangeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@', StringComparison.Ordinal))
        {
            throw new ArgumentException("A valid employee email is required.", nameof(email));
        }

        Email = email.Trim().ToLowerInvariant();
        MarkUpdated();
    }

    public void AssignDepartment(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
        {
            throw new ArgumentException("Department id is required.", nameof(departmentId));
        }

        DepartmentId = departmentId;
        MarkUpdated();
    }

    public void AssignRole(Guid roleId)
    {
        if (roleId == Guid.Empty)
        {
            throw new ArgumentException("Role id is required.", nameof(roleId));
        }

        RoleId = roleId;
        MarkUpdated();
    }
}
