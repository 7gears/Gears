namespace Gears.Application.Features.Roles.Update;

using Result = Results<
    NoContent,
    BadRequest,
    NotFound,
    UnprocessableEntity>;

public sealed class UpdateRole : Endpoint<UpdateRoleRequest, Result>
{
    private readonly RoleManager<Role> _roleManager;

    public UpdateRole(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public override void Configure()
    {
        Put("api/roles/{id}");
        AccessControl("Roles_Update", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(UpdateRoleRequest request, CancellationToken ct)
    {
        if (!HttpContext.User.HasPermission(Allow.Roles_ManagePermissions))
        {
            request = request with { Permissions = null };
        }

        if (!ValidatePermissions(request.Permissions))
        {
            return BadRequest();
        }

        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            return NotFound();
        }

        if (role.Name == Consts.Auth.RootRole)
        {
            return UnprocessableEntity();
        }

        var rolePermissions = await _roleManager.GetRolePermissionNames(role);
        ProcessPermissions(
            request,
            rolePermissions,
            out var permissionsToAdd,
            out var permissionsToDelete);

        role.Name = request.Name;
        role.Description = request.Description;
        role.IsDefault = request.IsDefault;

        var saveResult = await _roleManager.SaveRole(
            role,
            false,
            permissionsToAdd,
            permissionsToDelete);
        if (!saveResult)
        {
            return UnprocessableEntity();
        }

        return NoContent();
    }

    private static bool ValidatePermissions(IEnumerable<string> permissions) =>
        permissions == null ||
        permissions.All(Allow.AllNames().ToHashSet().Contains);

    private static void ProcessPermissions(
        UpdateRoleRequest request,
        HashSet<string> rolePermissions,
        out IEnumerable<string> permissionsToAdd,
        out IEnumerable<string> permissionsToDelete)
    {
        permissionsToAdd = null;
        permissionsToDelete = null;

        if (request.Permissions == null)
        {
            return;
        }

        permissionsToAdd = request.Permissions
            .Where(x => !rolePermissions.Contains(x));

        permissionsToDelete = rolePermissions
            .Where(x => !request.Permissions.Contains(x));
    }
}
