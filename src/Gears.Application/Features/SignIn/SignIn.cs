namespace Gears.Application.Features.SignIn;

public sealed record SignInRequest
(
    string Email,
    string Password
);

public sealed record SignInResponse
(
    string Token
);

public sealed class SignInValidator : Validator<SignInRequest>
{
    public SignInValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}

public sealed class SignInSwaggerSummary : Summary<SignIn>
{
    public SignInSwaggerSummary()
    {
        Response<SignInResponse>();
        Response(400);
        Response(401);
        Response(404);
    }
}

public sealed class SignIn : Endpoint<SignInRequest, Results<Ok<SignInResponse>, NotFound, UnauthorizedHttpResult>>
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
        Post("api/signin");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<SignInResponse>, NotFound, UnauthorizedHttpResult>> ExecuteAsync(SignInRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return NotFound();

        if(!user.EmailConfirmed)
            return Unauthorized();

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return Unauthorized();

        var token = await _jwtTokenProvider.GetToken(user);

        return Ok(new SignInResponse(token));
    }
}
