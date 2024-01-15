namespace Gears.Application.Features.Roles.Add;

using Result = Results<
    Created<AddRoleResponse>,
    BadRequest,
    UnprocessableEntity>;

public sealed class AddRoleEndpoint
(
    RoleManager<Role> roleManager
)
    : Endpoint<AddRoleRequest, Result>
{
    public override void Configure()
    {
        Post("api/roles");
        AccessControl("Roles_Add", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(AddRoleRequest request, CancellationToken ct)
    {
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

        var saveResult = await roleManager.SaveRoleWithPermissions(
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

    private static bool ValidatePermissions(IEnumerable<string> permissions)
    {
        return permissions == null || permissions.All(Allow.AllNames().ToHashSet().Contains);
    }
}
