namespace Gears.Application.Features.Roles.Update;

using Result = Results<
    NoContent,
    BadRequest,
    NotFound,
    UnprocessableEntity>;

public sealed class UpdateRole
(
    IHttpContextService httpContextService,
    RoleManager<Role> roleManager
)
    : Endpoint<UpdateRoleRequest, Result>
{
    public override void Configure()
    {
        Put("api/roles");
        AccessControl("Roles_Update", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(UpdateRoleRequest request, CancellationToken ct)
    {
        if (!httpContextService.HasPermission(Allow.Roles_ManagePermissions))
        {
            request = request with { Permissions = null };
        }

        if (!ValidatePermissions(request.Permissions))
        {
            return BadRequest();
        }

        var role = await roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            return NotFound();
        }

        if (role.Name == Consts.Auth.RootRole)
        {
            return UnprocessableEntity();
        }

        var permissions = await roleManager.GetRolePermissionNames(role);
        ProcessPermissions(
            request,
            permissions,
            out var permissionsToAdd,
            out var permissionsToDelete);

        role.Name = request.Name;
        role.Description = request.Description;
        role.IsDefault = request.IsDefault;

        var saveResult = await roleManager.SaveRole(
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
        HashSet<string> allPermissions,
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
            .Where(x => !allPermissions.Contains(x));

        permissionsToDelete = allPermissions
            .Where(x => !request.Permissions.Contains(x));
    }
}
