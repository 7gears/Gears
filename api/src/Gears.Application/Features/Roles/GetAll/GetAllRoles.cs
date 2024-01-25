namespace Gears.Application.Features.Roles.GetAll;

public sealed class GetAllRoles : EndpointWithoutRequest<List<GetAllRolesResponse>>
{
    private readonly RoleManager<Role> _roleManager;

    public GetAllRoles(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public override void Configure()
    {
        Get("api/roles");
        AccessControl("Roles_GetAll", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await _roleManager.Roles.AsNoTracking()
            .Where(x => x.Name != Consts.Auth.RootRoleName)
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
