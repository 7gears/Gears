namespace Gears.Application.Features.Roles.Get;

using GetRoleResult = Results<
    Ok<Response>,
    BadRequest,
    NotFound>;

public sealed class Endpoint
(
    RoleManager<Role> roleManager
)
    : Endpoint<Request, GetRoleResult>
{
    public override void Configure()
    {
        Get("api/roles/{id}");
        AccessControl("Roles_Get", Apply.ToThisEndpoint);
    }

    public override async Task<GetRoleResult> ExecuteAsync(Request request, CancellationToken ct)
    {
        var role = await roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            return NotFound();
        }

        var permissions = await roleManager.GetRolePermissionNames(role);

        var result = new Response(
            role.Id,
            role.Name,
            role.Description,
            role.IsDefault,
            permissions
        );

        return Ok(result);
    }
}
