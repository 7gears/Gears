namespace Gears.Application.Features.Account.ChangePassword;

using Result = Results<
    Ok,
    BadRequest,
    NotFound>;

public sealed class Endpoint
(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager
)
    : Endpoint<Request, Result>
{
    public override void Configure()
    {
        Post("api/account/change-password");
    }

    public override async Task<Result> ExecuteAsync(Request request, CancellationToken ct)
    {
        var userId = userManager.GetUserId(httpContextAccessor.HttpContext!.User);
        var user = await userManager.FindByIdAsync(userId!);
        if (user == null)
        {
            return NotFound();
        }

        var identityResult = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
        if (!identityResult.Succeeded)
        {
            return BadRequest();
        }

        return Ok();
    }
}
