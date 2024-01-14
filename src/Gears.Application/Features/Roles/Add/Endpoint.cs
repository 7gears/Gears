namespace Gears.Application.Features.Roles.Add;

using Result = Results<
    Created<Response>,
    BadRequest,
    UnprocessableEntity>;

public sealed class Endpoint
(
    RoleManager<Role> roleManager
)
    : Endpoint<Request, Result>
{
    public override void Configure()
    {
        Post("api/roles");
        AccessControl("Roles_Add", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(Request request, CancellationToken ct)
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

        return Created(string.Empty, new Response(role.Id));
    }

    private static bool ValidatePermissions(IEnumerable<string> permissions)
    {
        return permissions == null || permissions.All(Allow.AllNames().ToHashSet().Contains);
    }
}
