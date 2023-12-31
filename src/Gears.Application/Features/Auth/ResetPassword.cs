namespace Gears.Application.Features.Auth;

using ResetPasswordResultType = Results<Ok, NotFound, UnprocessableEntity>;

public sealed record ResetPasswordRequest(
    string Id,
    string Token,
    string Password
);

public sealed class ResetPasswordRequestValidator : Validator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}

public sealed class ResetPassword(
    UserManager<User> userManager,
    IPasswordHasher<User> passwordHasher
)
    : Endpoint<ResetPasswordRequest, ResetPasswordResultType>
{
    public override void Configure()
    {
        Post("api/reset-password");
        AllowAnonymous();
    }

    public override async Task<ResetPasswordResultType> ExecuteAsync(ResetPasswordRequest request, CancellationToken ct)
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

        if (!result)
        {
            return UnprocessableEntity();
        }

        var encryptedPassword = passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = encryptedPassword;
        await userManager.UpdateAsync(user);

        return Ok();
    }
}

public sealed class ResetPasswordSummary : Summary<ResetPassword>
{
    public ResetPasswordSummary()
    {
        Response();
        Response(400);
        Response(404);
        Response(422);
    }
}
