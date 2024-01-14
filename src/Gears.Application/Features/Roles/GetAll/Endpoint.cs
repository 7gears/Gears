namespace Gears.Application.Features.Roles.GetAll;

public sealed class Endpoint
(
    RoleManager<Role> roleManager
)
    : EndpointWithoutRequest<List<Response>>
{
    public override void Configure()
    {
        Get("api/roles");
        AccessControl("Roles_GetAll", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await roleManager.Roles.AsNoTracking()
            .Where(x => x.Name != Consts.Auth.RootRole)
            .OrderBy(x => x.Name)
            .Select(x => new Response(
                x.Id,
                x.Name,
                x.Description,
                x.IsDefault))
            .ToListAsync(ct);

        await SendAsync(result);
    }
}
