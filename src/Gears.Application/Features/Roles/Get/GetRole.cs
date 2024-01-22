namespace Gears.Application.Features.Roles.Get;

using Result = Results<
    Ok<GetRoleResponse>,
    BadRequest,
    NotFound>;

public sealed class GetRole : Endpoint<GetRoleRequest, Result>
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

    public override async Task<Result> ExecuteAsync(GetRoleRequest request, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            return NotFound();
        }

        var permissions = await _roleManager.GetRolePermissionNames(role);

        var response = new GetRoleResponse(
            role.Id,
            role.Name,
            role.Description,
            role.IsDefault,
            permissions
        );

        return Ok(response);
    }
}
