namespace Gears.Application.Features.Roles;

using UpdateRoleResult = Results<NoContent, BadRequest, NotFound, UnprocessableEntity>;

public sealed record UpdateRoleRequest(
    string Id,
    string Name,
    string Description,
    bool IsDefault,
    HashSet<string> Permissions);

public sealed class UpdateRoleValidator : Validator<UpdateRoleRequest>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}

public sealed class UpdateRole(
    RoleManager<Role> roleManager
)
    : Endpoint<UpdateRoleRequest, UpdateRoleResult>
{
    public override void Configure()
    {
        Put("api/roles");
        AccessControl("Roles_Update", Apply.ToThisEndpoint);
    }

    public override async Task<UpdateRoleResult> ExecuteAsync(UpdateRoleRequest request, CancellationToken ct)
    {
        if (!ValidatePermissions(request.Permissions))
            return BadRequest();

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

        var permissionsToAdd = request.Permissions
            .Where(x => !permissions.Contains(x));

        var permissionsToRemove = permissions
            .Where(x => !request.Permissions.Contains(x));

        role.Name = request.Name;
        role.Description = request.Description;
        role.IsDefault = request.IsDefault;

        var result = await roleManager.SaveRoleWithPermissions(
            role,
            false,
            permissionsToAdd,
            permissionsToRemove);
        if (!result)
        {
            return UnprocessableEntity();
        }

        return NoContent();
    }

    private static bool ValidatePermissions(IEnumerable<string> permissions) =>
        permissions == null || permissions.All(Allow.AllNames().ToHashSet().Contains);
}
