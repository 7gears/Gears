namespace Gears.Application.Features.Account.ChangePassword;

using Result = Results<
    Ok,
    BadRequest,
    NotFound>;

public sealed class ChangePassword : Endpoint<ChangePasswordRequest, Result>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;

    public ChangePassword(
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("api/account/change-password");
    }

    public override async Task<Result> ExecuteAsync(ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext!.User);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
        {
            return NotFound();
        }

        var identityResult = await _userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
        if (!identityResult.Succeeded)
        {
            return BadRequest();
        }

        return Ok();
    }
}
