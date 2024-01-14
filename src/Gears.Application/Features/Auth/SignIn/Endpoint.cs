namespace Gears.Application.Features.Auth.SignIn;

using Result = Results<
    Ok<Response>,
    NotFound,
    UnauthorizedHttpResult>;

public sealed class Endpoint
(
    UserManager<User> userManager,
    IJwtTokenProvider jwtTokenProvider
)
    : Endpoint<Request, Result>
{
    public override void Configure()
    {
        Post("api/auth/signin");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(Request request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is not { IsActive: true })
        {
            return NotFound();
        }

        if (!user.EmailConfirmed)
        {
            return Unauthorized();
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return Unauthorized();
        }

        var token = await jwtTokenProvider.GetToken(user);

        return Ok(new Response(token));
    }
}
