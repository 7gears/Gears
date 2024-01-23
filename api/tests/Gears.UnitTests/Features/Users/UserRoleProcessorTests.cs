namespace Gears.UnitTests.Features.Users;

public sealed class UserRoleProcessorTests
{
    private static readonly Role AdminRole = new() { Name = "admin" };
    private static readonly Role ViewerRole = new() { Name = "viewer", IsDefault = true };

    [Fact]
    public void EmptyRequest()
    {
        var result = UpdateUserRequestRoleParser.TryParseRoles(
            new([], [], []),
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
            new(["UnknownRoleId"], [AdminRole, ViewerRole], []),
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
            new([AdminRole.Id], [AdminRole, ViewerRole], []),
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
            new([AdminRole.Id, ViewerRole.Id], [AdminRole, ViewerRole], ["viewer"]),
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
            new([ViewerRole.Id], [AdminRole, ViewerRole], ["viewer", "admin"]),
            out var rolesToAdd,
            out var rolesToDelete);

        Assert.True(result);
        Assert.Empty(rolesToAdd);
        Assert.Contains("admin", rolesToDelete);
    }
}
