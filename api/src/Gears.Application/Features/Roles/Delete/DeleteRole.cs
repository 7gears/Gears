namespace Gears.Application.Features.Roles.Delete;

public sealed class DeleteRole : Endpoint<DeleteRoleRequest>
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

    public override async Task HandleAsync(DeleteRoleRequest request, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            await SendNotFoundAsync();
            return;
        }

        if (role.Name == Consts.Auth.RootRoleName)
        {
            await SendErrorsAsync();
            return;
        }

        var identityResult = await _roleManager.DeleteAsync(role);
        if (identityResult != IdentityResult.Success)
        {
            await SendErrorsAsync();
            return;
        }

        await SendNoContentAsync();
    }
}
