namespace Application.Features.Roles.Get;

public sealed class GetRole : Endpoint<GetRoleRequest>
{
    private readonly RoleManager<Role> _roleManager;

    public GetRole(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public override void Configure()
    {
        Get("api/roles/{id}");
        AccessControl("Roles_Get", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(GetRoleRequest request, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            await SendNotFoundAsync();
            return;
        }

        var permissions = await _roleManager.GetRolePermissionNames(role);

        var response = new GetRoleResponse(
            role.Id,
            role.Name,
            role.Description,
            role.IsDefault,
            permissions
        );

        await SendAsync(response);
    }
}
