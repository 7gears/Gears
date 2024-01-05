namespace Gears.Application.Features.Account;

using UpdateProfileResponseResult = Results<Ok, NotFound>;

public sealed record UpdateProfileRequest(
    string FirstName,
    string LastName,
    string PhoneNumber
);

public sealed class UpdateProfile(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager
)
    : Endpoint<UpdateProfileRequest, UpdateProfileResponseResult>
{
    public override void Configure()
    {
        Patch("api/account/profile");
    }

    public override async Task<UpdateProfileResponseResult> ExecuteAsync(UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = userManager.GetUserId(httpContextAccessor.HttpContext!.User);
        var user = await userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        await userManager.UpdateAsync(user);

        return Ok();
    }
}
