namespace Gears.Application.Features.Account;

using GetProfileResponseResultType = Results<Ok<GetProfileResponse>, NotFound>;

public sealed record GetProfileResponse(
    string FirstName,
    string LastName
);

public sealed class GetProfile(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager
)
    : EndpointWithoutRequest<GetProfileResponseResultType>
{
    public override void Configure()
    {
        Get("api/account/profile");
    }

    public override async Task<GetProfileResponseResultType> ExecuteAsync(CancellationToken ct)
    {
        var userName = userManager.GetUserName(httpContextAccessor.HttpContext!.User);

        var user = await userManager.Users.AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserName == userName, ct);

        if (user == null)
        {
            return NotFound();
        }

        var response = new GetProfileResponse(user.FirstName, user.LastName);

        return Ok(response);
    }
}
