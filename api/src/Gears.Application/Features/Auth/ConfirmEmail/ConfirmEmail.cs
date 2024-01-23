namespace Gears.Application.Features.Auth.ConfirmEmail;

public sealed class ConfirmEmail : Endpoint<ConfirmEmailRequest>
{
    private readonly UserManager<User> _userManager;

    public ConfirmEmail(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("api/auth/confirm-email");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ConfirmEmailRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user is not { IsActive: true })
        {
            await SendNotFoundAsync();
            return;
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            await SendErrorsAsync();
            return;
        }

        await SendOkAsync();
    }
}
