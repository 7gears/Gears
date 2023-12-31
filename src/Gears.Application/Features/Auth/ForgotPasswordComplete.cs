namespace Gears.Application.Features.Auth;

using ForgotPasswordCompleteResultType = Results<Ok, NotFound, UnprocessableEntity>;

public sealed record ForgotPasswordCompleteRequest(
    string Id,
    string Token,
    string Password
);

public sealed class ForgotPasswordCompleteRequestValidator : Validator<ForgotPasswordCompleteRequest>
{
    public ForgotPasswordCompleteRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required");
    }
}

public sealed class ForgotPasswordComplete(
    UserManager<User> userManager,
    IPasswordHasher<User> passwordHasher
)
    : Endpoint<ForgotPasswordCompleteRequest, ForgotPasswordCompleteResultType>
{
    public override void Configure()
    {
        Post("api/forgot-password-complete");
        AllowAnonymous();
    }

    public override async Task<ForgotPasswordCompleteResultType> ExecuteAsync(ForgotPasswordCompleteRequest request,
        CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await userManager.VerifyUserTokenAsync(
            user,
            userManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<User>.ResetPasswordTokenPurpose,
            request.Token);

        if (result == false)
        {
            return UnprocessableEntity();
        }

        var encryptedPassword = passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = encryptedPassword;
        await userManager.UpdateAsync(user);

        return Ok();
    }
}

public sealed class ForgotPasswordCompleteSummary : Summary<ForgotPasswordComplete>
{
    public ForgotPasswordCompleteSummary()
    {
        Response();
        Response(400);
        Response(404);
        Response(422);
    }
}
