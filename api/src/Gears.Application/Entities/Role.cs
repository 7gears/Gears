namespace Gears.Application.Entities;

public sealed class Role : IdentityRole
{
    public string Description { get; set; }

    public bool IsDefault { get; set; }
}
