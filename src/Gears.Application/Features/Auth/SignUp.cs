namespace Gears.Application.Features.Auth;

using SignUpResponseResultType = Results<Created<SignUpResponse>, Conflict>;

public sealed record SignUpRequest(
    string Email,
    string Password
);

public sealed record SignUpResponse(
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

public sealed class SignUp(
    UserManager<User> userManager,
    IPasswordHasher<User> passwordHasher,
    IMailService mailService,
    IHttpContextService httpContextService
)
    : Endpoint<SignUpRequest, SignUpResponseResultType>
{
    public override void Configure()
    {
        Post("api/auth/signup");
        AllowAnonymous();
    }

    public override async Task<SignUpResponseResultType> ExecuteAsync(SignUpRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user != null)
        {
            return Conflict();
        }

        user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = false,
            PasswordHash = passwordHasher.HashPassword(null!, request.Password)
        };

        await userManager.CreateAsync(user);

        var link = await GenerateConfirmEmailLink(user);
        var mailRequest = new MailRequest(user.Email, "Confirm Email", link);

        _ = mailService.Send(mailRequest);

        return Created(string.Empty, new SignUpResponse(user.Id));
    }

    private async Task<string> GenerateConfirmEmailLink(User user)
    {
        var origin = httpContextService.GetOrigin();
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        UriBuilder builder = new(origin)
        {
            Path = "confirm-email"
        };
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["Id"] = user.Id;
        query["Token"] = token;
        builder.Query = query.ToString()!;

        return builder.ToString();
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
