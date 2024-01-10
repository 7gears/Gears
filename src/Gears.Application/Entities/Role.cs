namespace Gears.Application.Entities;

public sealed class Role : IdentityRole, IDeletable
{
    public string Description { get; set; }

    public bool IsDeletable { get; set; }

    public bool IsDefault { get; set; }
}
