namespace Gears.Application.Features.Auth.SignUp;

using Result = Results<
    Created<SignUpResponse>,
    BadRequest,
    UnprocessableEntity>;

public sealed class SignUpEndpoint
(
    UserManager<User> userManager,
    IPasswordHasher<User> passwordHasher,
    IMailService mailService,
    IHttpContextService httpContextService
)
    : Endpoint<SignUpRequest, Result>
{
    public override void Configure()
    {
        Post("api/auth/signup");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(SignUpRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user != null)
        {
            return UnprocessableEntity();
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

        var identityResult = await userManager.CreateAsync(user);
        if (identityResult != IdentityResult.Success)
        {
            return UnprocessableEntity();
        }

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
