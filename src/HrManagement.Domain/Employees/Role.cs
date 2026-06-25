using HrManagement.Domain.Common;

namespace HrManagement.Domain.Employees;

public sealed class Role : Entity
{
    private Role()
    {
    }

    public Role(string title)
    {
        Rename(title);
    }

    public string Title { get; private set; } = string.Empty;

    public void Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Role title is required.", nameof(title));
        }

        Title = title.Trim();
        MarkUpdated();
    }
}
