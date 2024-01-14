namespace Gears.Application.Features.Auth.ConfirmEmail;

using Result = Results<
    Ok,
    NotFound,
    UnprocessableEntity>;

public sealed class Endpoint
(
    UserManager<User> userManager
)
    : Endpoint<Request, Result>
{
    public override void Configure()
    {
        Post("api/auth/confirm-email");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(
        Request request,
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
