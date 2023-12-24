namespace Gears.Application.Features.Auth;

using SignUpResponseResultType = Results<Created<SignUpResponse>, Conflict>;

public sealed record SignUpRequest
(
    string Email,
    string Password
);

public sealed record SignUpResponse
(
    string Id
);

public sealed class SignUpRequestValidator : Validator<SignUpRequest>
{
    public SignUpRequestValidator()
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

public sealed class SignUp : Endpoint<SignUpRequest, SignUpResponseResultType>
{
    private readonly UserManager<User> _userManager;
    private readonly IPasswordHasher<User> _passwordHasher;

    public SignUp(
        UserManager<User> userManager,
        IPasswordHasher<User> passwordHasher)
    {
        _userManager = userManager;
        _passwordHasher = passwordHasher;
    }

    public override void Configure()
    {
        Post("api/signup");
        AllowAnonymous();
    }

    public override async Task<SignUpResponseResultType> ExecuteAsync(SignUpRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user != null)
            return Conflict();

        user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = false,
            PasswordHash = _passwordHasher.HashPassword(null!, request.Password)
        };

        await _userManager.CreateAsync(user);

        return Created(string.Empty, new SignUpResponse(string.Empty));
    }
}

public sealed class SignUpSwaggerSummary : Summary<SignUp>
{
    public SignUpSwaggerSummary()
    {
        Response<SignUpResponse>(201);
        Response(400);
        Response(409);
    }
}
