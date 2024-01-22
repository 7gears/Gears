namespace Gears.Application.Features.Account.UpdateProfile;

using Result = Results<
    Ok,
    NotFound,
    UnprocessableEntity>;

public sealed class UpdateProfile : Endpoint<UpdateProfileRequest, Result>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;

    public UpdateProfile(
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Patch("api/account/profile");
    }

    public override async Task<Result> ExecuteAsync(UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext!.User);
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
