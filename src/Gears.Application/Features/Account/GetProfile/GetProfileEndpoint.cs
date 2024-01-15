namespace Gears.Application.Features.Account.GetProfile;

using Result = Results<
    Ok<GetProfileResponse>,
    NotFound>;

public sealed class GetProfileEndpoint
(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager
)
    : EndpointWithoutRequest<Result>
{
    public override void Configure()
    {
        Get("api/account/profile");
    }

    public override async Task<Result> ExecuteAsync(CancellationToken ct)
    {
        var userId = userManager.GetUserId(httpContextAccessor.HttpContext!.User);
        var user = await userManager.FindByIdAsync(userId!);
        if (user == null)
        {
            return NotFound();
        }

        var response = new GetProfileResponse(
            user.FirstName,
            user.LastName,
            user.PhoneNumber);

        return Ok(response);
    }
}
