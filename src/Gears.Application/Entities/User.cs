namespace Gears.Application.Entities;

public sealed class User : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}