namespace Gears.Application.Features.Auth.SignIn;

using Result = Results<
    Ok<SignInResponse>,
    NotFound,
    UnauthorizedHttpResult>;

public sealed class SignIn : Endpoint<SignInRequest, Result>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public SignIn(UserManager<User> userManager,
        IJwtTokenProvider jwtTokenProvider)
    {
        _userManager = userManager;
        _jwtTokenProvider = jwtTokenProvider;
    }

    public override void Configure()
    {
        Post("api/auth/signin");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(SignInRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is not { IsActive: true })
        {
            return NotFound();
        }

        if (!user.EmailConfirmed)
        {
            return Unauthorized();
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return Unauthorized();
        }

        var token = await _jwtTokenProvider.GetToken(user);

        return Ok(new SignInResponse(token));
    }
}
