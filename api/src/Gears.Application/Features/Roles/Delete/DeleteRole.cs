namespace Gears.Application.Features.Roles.Delete;

using Result = Results<
    Ok,
    BadRequest,
    NotFound,
    UnprocessableEntity>;

public sealed class DeleteRole : Endpoint<DeleteRoleRequest, Result>
{
    private readonly RoleManager<Role> _roleManager;

    public DeleteRole(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public override void Configure()
    {
        Delete("api/roles/{id}");
        AccessControl("Roles_Delete", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(DeleteRoleRequest request, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            return NotFound();
        }

        if (role.Name == Consts.Auth.RootRole)
        {
            return UnprocessableEntity();
        }

        var identityResult = await _roleManager.DeleteAsync(role);
        if (identityResult != IdentityResult.Success)
        {
            return UnprocessableEntity();
        }

        return Ok();
    }
}
