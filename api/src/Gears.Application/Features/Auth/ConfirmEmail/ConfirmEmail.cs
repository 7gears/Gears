namespace Gears.Application.Features.Auth.ConfirmEmail;

using Result = Results<
    Ok,
    NotFound,
    UnprocessableEntity>;

public sealed class ConfirmEmail : Endpoint<ConfirmEmailRequest, Result>
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

    public override async Task<Result> ExecuteAsync(
        ConfirmEmailRequest request,
        CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user is not { IsActive: true })
        {
            return NotFound();
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        return result.Succeeded ? Ok() : UnprocessableEntity();
    }
}
