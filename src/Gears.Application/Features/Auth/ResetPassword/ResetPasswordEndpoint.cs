namespace Gears.Application.Features.Auth.ResetPassword;

using Result = Results<
    Ok,
    BadRequest,
    NotFound>;

public sealed class ResetPasswordEndpoint
(
    UserManager<User> userManager,
    IPasswordHasher<User> passwordHasher
)
    : Endpoint<ResetPasswordRequest, Result>
{
    public override void Configure()
    {
        Post("api/auth/reset-password");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(ResetPasswordRequest request, CancellationToken ct)
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
