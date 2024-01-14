namespace Gears.Application.Features.Users.GetAll;

public sealed class Endpoint
(
    UserManager<User> userManager
)
    : EndpointWithoutRequest<List<Response>>
{
    public override void Configure()
    {
        Get("api/users");
        AccessControl("Users_GetAll", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await userManager.Users.AsNoTracking()
            .Where(x => x.UserName != Consts.Auth.RootUser)
            .Select(x => new Response(
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
