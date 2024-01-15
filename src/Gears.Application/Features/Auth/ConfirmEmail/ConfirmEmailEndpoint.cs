namespace Gears.Application.Features.Auth.ConfirmEmail;

using Result = Results<
    Ok,
    NotFound,
    UnprocessableEntity>;

public sealed class ConfirmEmailEndpoint
(
    UserManager<User> userManager
)
    : Endpoint<ConfirmEmailRequest, Result>
{
    public override void Configure()
    {
        Post("api/auth/confirm-email");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(
        ConfirmEmailRequest request,
        CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id);
        if (user is not { IsActive: true })
        {
            return NotFound();
        }

        var result = await userManager.ConfirmEmailAsync(user, request.Token);

        return result.Succeeded ? Ok() : UnprocessableEntity();
    }
}
