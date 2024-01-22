namespace Gears.Application.Features.Account.UpdateProfile;

using Result = Results<
    Ok,
    NotFound,
    UnprocessableEntity>;

public sealed class UpdateProfile : Endpoint<UpdateProfileRequest, Result>
{
    private readonly UserManager<User> _userManager;

    public UpdateProfile(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Patch("api/account/profile");
    }

    public override async Task<Result> ExecuteAsync(UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = _userManager.GetUserId(HttpContext.User);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
        {
            return NotFound();
        }

        user.UserName = request.UserName;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
        {
            return UnprocessableEntity();
        }

        return Ok();
    }
}
