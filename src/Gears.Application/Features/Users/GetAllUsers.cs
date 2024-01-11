namespace Gears.Application.Features.Users;

public sealed record GetAllUsersRequest;

public sealed record GetAllUsersResponse(
    string Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    bool IsActive);

public sealed class GetAllUsers(
    IApplicationDbContext db
)
    : Endpoint<GetAllUsersRequest, List<GetAllUsersResponse>>
{
    public override void Configure()
    {
        Get("api/users");
        AccessControl("Users-GetAll", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(GetAllUsersRequest req, CancellationToken ct)
    {
        var result = await db.Users.AsNoTracking()
            .Where(x => x.UserName != Consts.Auth.RootUser)
            .Select(x => new GetAllUsersResponse(
                x.Id,
                x.UserName,
                x.Email,
                x.FirstName,
                x.LastName,
                x.PhoneNumber,
                x.IsActive))
            .ToListAsync(ct);

        await SendAsync(result);
    }
}
