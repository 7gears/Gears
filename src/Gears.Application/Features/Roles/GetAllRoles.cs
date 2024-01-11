namespace Gears.Application.Features.Roles;

public sealed record GetAllRolesRequest;

public sealed record GetAllRolesResponse(
    string Id,
    string Name,
    string Description,
    bool IsDeletable,
    bool IsDefault
);

public sealed class GetAllRoles(
    IApplicationDbContext db
)
    : Endpoint<GetAllRolesRequest, List<GetAllRolesResponse>>
{
    public override void Configure()
    {
        Get("api/roles");
        AccessControl("Roles-GetAll", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(GetAllRolesRequest req, CancellationToken ct)
    {
        var result = await db.Roles.AsNoTracking()
            .Select(x => new GetAllRolesResponse(
                x.Id,
                x.Name,
                x.Description,
                x.IsDeletable,
                x.IsDefault))
            .ToListAsync(ct);

        await SendAsync(result);
    }
}
