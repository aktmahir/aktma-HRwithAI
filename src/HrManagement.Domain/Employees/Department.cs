using HrManagement.Domain.Common;

namespace HrManagement.Domain.Employees;

public sealed class Department : Entity
{
    private Department()
    {
    }

    public Department(string name)
    {
        Rename(name);
    }

    public string Name { get; private set; } = string.Empty;

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Department name is required.", nameof(name));
        }

        Name = name.Trim();
        MarkUpdated();
    }
}
