namespace Application.Features.Account.ChangePassword;

public sealed class ChangePassword : Endpoint<ChangePasswordRequest>
{
    private readonly UserManager<User> _userManager;

    public ChangePassword(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("api/account/change-password");
    }

    public override async Task HandleAsync(ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = _userManager.GetUserId(HttpContext.User);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
        {
            await SendNotFoundAsync();
            return;
        }

        var identityResult = await _userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
        if (!identityResult.Succeeded)
        {
            await SendErrorsAsync();
            return;
        }

        await SendOkAsync();
    }
}
