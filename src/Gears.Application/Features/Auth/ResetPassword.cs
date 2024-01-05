namespace Gears.Application.Features.Auth;

using ResetPasswordResult = Results<Ok, BadRequest, NotFound>;

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
    : Endpoint<ResetPasswordRequest, ResetPasswordResult>
{
    public override void Configure()
    {
        Post("api/auth/reset-password");
        AllowAnonymous();
    }

    public override async Task<ResetPasswordResult> ExecuteAsync(ResetPasswordRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id);
        if (user is not { IsActive: true })
        {
            return NotFound();
        }

        foreach (var passwordValidator in userManager.PasswordValidators)
        {
            var passwordValidationResult = await passwordValidator.ValidateAsync(userManager, user, request.Password);
            if (passwordValidationResult != IdentityResult.Success)
            {
                return BadRequest();
            }
        }

        var tokenValidationResult = await userManager.VerifyUserTokenAsync(
            user,
            userManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<User>.ResetPasswordTokenPurpose,
            request.Token);

        if (!tokenValidationResult)
        {
            return BadRequest();
        }

        var encryptedPassword = passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = encryptedPassword;
        await userManager.UpdateAsync(user);

        return Ok();
    }
}
