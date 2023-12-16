namespace Gears.Application.Features.Login;

public sealed record LoginRequest
(
    string Username,
    string Password
);

public sealed record LoginResponse
(
    string Token
);

public sealed class Login
(
    UserManager<User> _userManager,
    IJwtTokenProvider _jwtTokenProvider
) : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Get("api/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            throw null; //TODO:
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            throw null; //TODO:
        }

        var token = await _jwtTokenProvider.GetToken(user);

        await SendAsync(new LoginResponse(token));
    }

}