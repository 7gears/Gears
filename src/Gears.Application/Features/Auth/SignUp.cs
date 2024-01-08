namespace Gears.Application.Features.Auth;

using SignUpResult = Results<Created<SignUpResponse>, BadRequest, Conflict>;

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
    : Endpoint<SignUpRequest, SignUpResult>
{
    public override void Configure()
    {
        Post("api/auth/signup");
        AllowAnonymous();
    }

    public override async Task<SignUpResult> ExecuteAsync(SignUpRequest request, CancellationToken ct)
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
            IsActive = true,
            PasswordHash = passwordHasher.HashPassword(null!, request.Password)
        };

        foreach (var passwordValidator in userManager.PasswordValidators)
        {
            var passwordValidationResult = await passwordValidator.ValidateAsync(userManager, user, request.Password);
            if (passwordValidationResult != IdentityResult.Success)
            {
                return BadRequest();
            }
        }

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
