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

public sealed class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}

public sealed class LoginSwaggerSummary : Summary<Login>
{
    public LoginSwaggerSummary()
    {
        Response<LoginResponse>();
        Response(400);
        Response(401);
        Response(404);
    }
}

public sealed class Login : Endpoint<LoginRequest, Results<Ok<LoginResponse>, NotFound, UnauthorizedHttpResult>>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public Login(
        UserManager<User> userManager,
        IJwtTokenProvider jwtTokenProvider)
    {
        _userManager = userManager;
        _jwtTokenProvider = jwtTokenProvider;
    }

    public override void Configure()
    {
        Post("api/login");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<LoginResponse>, NotFound, UnauthorizedHttpResult>> ExecuteAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            return NotFound();

        if(!user.EmailConfirmed)
            return Unauthorized();

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return Unauthorized();

        var token = await _jwtTokenProvider.GetToken(user);

        return Ok(new LoginResponse(token));
    }
}
