namespace Application.Features.Auth.SignIn;

public sealed class SignIn : Endpoint<SignInRequest>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public SignIn(
        UserManager<User> userManager,
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

    public override async Task HandleAsync(SignInRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is not { IsActive: true })
        {
            await SendNotFoundAsync();
            return;
        }

        if (!user.EmailConfirmed)
        {
            await SendForbiddenAsync();
            return;
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            await SendForbiddenAsync();
            return;
        }

        var token = await _jwtTokenProvider.GetToken(user);
        var response = new SignInResponse(token);

        await SendOkAsync(response);
    }
}
