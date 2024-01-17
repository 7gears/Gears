namespace Gears.UnitTests.Features.Users;

public sealed class UserRoleProcessorTests
{
    private static readonly UpdateUserRequest Empty = new(
        default,
        default,
        default,
        default,
        default,
        default,
        default,
        default);

    [Fact]
    public void EmptyRequest()
    {
        var result = UserRoleProcessor.TryParseRoles(Empty, null, null, out var rolesToAdd, out var rolesToDelete);

        Assert.True(result);
        Assert.Null(rolesToAdd);
        Assert.Null(rolesToDelete);
    }

    [Fact]
    public void RoleDoesNotExist()
    {
        var request = Empty with { RoleIds = new[] { "UnknownRole" } };
        var allRoles = new List<Role> { new() { Name = "admin" }, new() { Name = "viewer" } };

        var result = UserRoleProcessor.TryParseRoles(request, allRoles, null, out var rolesToAdd, out var rolesToDelete);

        Assert.False(result);
        Assert.Null(rolesToAdd);
        Assert.Null(rolesToDelete);
    }

    [Fact]
    public void AssignDefaultRole_IfItDoesNotExist()
    {
        var adminRole = new Role { Name = "admin" };
        var viewerRole = new Role { Name = "viewer", IsDefault = true };

        var request = Empty with { RoleIds = [] };
        var allRoles = new List<Role> { adminRole, viewerRole };
        var userRoles = new List<string>();

        var result = UserRoleProcessor.TryParseRoles(request, allRoles, userRoles, out var rolesToAdd, out var rolesToDelete);

        Assert.True(result);
        Assert.Contains("viewer", rolesToAdd);
        Assert.Empty(rolesToDelete);
    }

    [Fact]
    public void SkipDefaultRole_IfItExists()
    {
        var adminRole = new Role { Name = "admin" };
        var viewerRole = new Role { Name = "viewer", IsDefault = true };

        var request = Empty with { RoleIds = [] };
        var allRoles = new List<Role> { adminRole, viewerRole };
        var userRoles = new List<string> { "viewer" };

        var result = UserRoleProcessor.TryParseRoles(request, allRoles, userRoles, out var rolesToAdd, out var rolesToDelete);

        Assert.True(result);
        Assert.Empty(rolesToAdd);
        Assert.Empty(rolesToDelete);
    }

    [Fact]
    public void AddRole()
    {
        var adminRole = new Role { Name = "admin" };
        var viewerRole = new Role { Name = "viewer", IsDefault = true };

        var request = Empty with { RoleIds = [adminRole.Id] };
        var allRoles = new List<Role> { adminRole, viewerRole };
        var userRoles = new List<string> { "viewer" };

        var result = UserRoleProcessor.TryParseRoles(request, allRoles, userRoles, out var rolesToAdd, out var rolesToDelete);

        Assert.True(result);
        Assert.Contains("admin", rolesToAdd);
        Assert.Empty(rolesToDelete);
    }

    [Fact]
    public void DeleteRole()
    {
        var adminRole = new Role { Name = "admin" };
        var viewerRole = new Role { Name = "viewer", IsDefault = true };

        var request = Empty with { RoleIds = [viewerRole.Id] };
        var allRoles = new List<Role> { adminRole, viewerRole };
        var userRoles = new List<string> { "viewer", "admin" };

        var result = UserRoleProcessor.TryParseRoles(request, allRoles, userRoles, out var rolesToAdd, out var rolesToDelete);

        Assert.True(result);
        Assert.Empty(rolesToAdd);
        Assert.Contains("admin", rolesToDelete);
    }
}
