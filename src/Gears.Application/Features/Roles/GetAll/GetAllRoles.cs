namespace Gears.Application.Features.Roles.GetAll;

public sealed class GetAllRoles
(
    RoleManager<Role> roleManager
)
    : EndpointWithoutRequest<List<GetAllRolesResponse>>
{
    public override void Configure()
    {
        Get("api/roles");
        AccessControl("Roles_GetAll", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await roleManager.Roles.AsNoTracking()
            .Where(x => x.Name != Consts.Auth.RootRole)
            .OrderBy(x => x.Name)
            .Select(x => new GetAllRolesResponse(
                x.Id,
                x.Name,
                x.Description,
                x.IsDefault))
            .ToListAsync(ct);

        await SendAsync(response);
    }
}
