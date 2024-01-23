namespace Gears.UnitTests.Features.Users;

public sealed class UserRoleProcessorTests
{
    private static readonly Role AdminRole = new() { Name = "admin" };
    private static readonly Role ViewerRole = new() { Name = "viewer", IsDefault = true };

    [Fact]
    public void EmptyRequest()
    {
        var result = UpdateUserRequestRoleParser.TryParseRoles(
            new(new HashSet<string>(),
                new List<Role>(),
                new List<string>()),
            out var rolesToAdd,
            out var rolesToDelete);

        Assert.True(result);
        Assert.Empty(rolesToAdd);
        Assert.Empty(rolesToDelete);
    }

    [Fact]
    public void RoleDoesNotExist()
    {
        var result = UpdateUserRequestRoleParser.TryParseRoles(
            new(new HashSet<string> { "UnknownRoleId" },
                new List<Role> { AdminRole, ViewerRole },
                new List<string>()),
            out var rolesToAdd,
            out var rolesToDelete);

        Assert.False(result);
        Assert.Empty(rolesToAdd);
        Assert.Empty(rolesToDelete);
    }

    [Fact]
    public void DefaultRoleDoesNotExist()
    {
        var result = UpdateUserRequestRoleParser.TryParseRoles(
            new(new HashSet<string> { AdminRole.Id },
                new List<Role> { AdminRole, ViewerRole },
                new List<string>()),
            out var rolesToAdd,
            out var rolesToDelete);

        Assert.False(result);
        Assert.Empty(rolesToAdd);
        Assert.Empty(rolesToDelete);
    }

    [Fact]
    public void AddRole()
    {
        var result = UpdateUserRequestRoleParser.TryParseRoles(
            new(new HashSet<string> { AdminRole.Id, ViewerRole.Id },
                new List<Role> { AdminRole, ViewerRole },
                new List<string> { "viewer" }),
            out var rolesToAdd,
            out var rolesToDelete);

        Assert.True(result);
        Assert.Contains("admin", rolesToAdd);
        Assert.Empty(rolesToDelete);
    }

    [Fact]
    public void DeleteRole()
    {
        var result = UpdateUserRequestRoleParser.TryParseRoles(
            new(new HashSet<string> { ViewerRole.Id },
                new List<Role> { AdminRole, ViewerRole },
                new List<string> { "viewer", "admin" }),
            out var rolesToAdd,
            out var rolesToDelete);

        Assert.True(result);
        Assert.Empty(rolesToAdd);
        Assert.Contains("admin", rolesToDelete);
    }
}
