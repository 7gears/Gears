namespace Gears.Application.Features.Roles.Delete;

using Result = Results<
    Ok,
    BadRequest,
    NotFound,
    UnprocessableEntity>;

public sealed class DeleteRole
(
    RoleManager<Role> roleManager
)
    : Endpoint<DeleteRoleRequest, Result>
{
    public override void Configure()
    {
        Delete("api/roles");
        AccessControl("Roles_Delete", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(DeleteRoleRequest request, CancellationToken ct)
    {
        var role = await roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            return NotFound();
        }

        if (role.Name == Consts.Auth.RootRole)
        {
            return UnprocessableEntity();
        }

        var identityResult = await roleManager.DeleteAsync(role);
        if (identityResult != IdentityResult.Success)
        {
            return UnprocessableEntity();
        }

        return Ok();
    }
}
