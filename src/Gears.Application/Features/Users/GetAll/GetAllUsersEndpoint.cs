namespace Gears.Application.Features.Users.GetAll;

public sealed class GetAllUsersEndpoint
(
    UserManager<User> userManager
)
    : EndpointWithoutRequest<List<GetAllUsersResponse>>
{
    public override void Configure()
    {
        Get("api/users");
        AccessControl("Users_GetAll", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await userManager.Users.AsNoTracking()
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

        await SendAsync(response);
    }
}
