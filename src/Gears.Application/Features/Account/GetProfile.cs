namespace Gears.Application.Features.Account;

using GetProfileResult = Results<Ok<GetProfileResponse>, NotFound>;

public sealed record GetProfileResponse(
    string FirstName,
    string LastName,
    string PhoneNumber
);

public sealed class GetProfile(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager
)
    : EndpointWithoutRequest<GetProfileResult>
{
    public override void Configure()
    {
        Get("api/account/profile");
    }

    public override async Task<GetProfileResult> ExecuteAsync(CancellationToken ct)
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
