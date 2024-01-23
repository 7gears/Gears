namespace Gears.Application.Features.Roles.Add;

using Result = Results<
    Created<AddRoleResponse>,
    BadRequest,
    UnprocessableEntity>;

public sealed class AddRole : Endpoint<AddRoleRequest, Result>
{
    private readonly RoleManager<Role> _roleManager;

    public AddRole(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public override void Configure()
    {
        Post("api/roles");
        AccessControl("Roles_Add", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(AddRoleRequest request, CancellationToken ct)
    {
        if (!HttpContext.User.HasPermission(Allow.Roles_ManagePermissions))
        {
            request = request with { Permissions = null };
        }

        if (!ValidatePermissions(request.Permissions))
        {
            return BadRequest();
        }

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description,
            IsDefault = request.IsDefault
        };

        var saveResult = await _roleManager.SaveRole(
            role,
            true,
            request.Permissions,
            null);

        if (!saveResult)
        {
            return UnprocessableEntity();
        }

        return Created(string.Empty, new AddRoleResponse(role.Id));
    }

    private static bool ValidatePermissions(IEnumerable<string> permissions) =>
        permissions == null ||
        permissions.All(Allow.AllNames().ToHashSet().Contains);
}
