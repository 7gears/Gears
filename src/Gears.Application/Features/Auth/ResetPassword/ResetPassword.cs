namespace Gears.Application.Features.Auth.ResetPassword;

using Result = Results<
    Ok,
    BadRequest,
    NotFound>;

public sealed class ResetPassword : Endpoint<ResetPasswordRequest, Result>
{
    private readonly UserManager<User> _userManager;
    private readonly IPasswordHasher<User> _passwordHasher;

    public ResetPassword(UserManager<User> userManager,
        IPasswordHasher<User> passwordHasher)
    {
        _userManager = userManager;
        _passwordHasher = passwordHasher;
    }

    public override void Configure()
    {
        Post("api/auth/reset-password");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(ResetPasswordRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user is not { IsActive: true })
        {
            return NotFound();
        }

        foreach (var passwordValidator in _userManager.PasswordValidators)
        {
            var passwordValidationResult = await passwordValidator.ValidateAsync(_userManager, user, request.Password);
            if (passwordValidationResult != IdentityResult.Success)
            {
                return BadRequest();
            }
        }

        var tokenValidationResult = await _userManager.VerifyUserTokenAsync(
            user,
            _userManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<User>.ResetPasswordTokenPurpose,
            request.Token);

        if (!tokenValidationResult)
        {
            return BadRequest();
        }

        var encryptedPassword = _passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = encryptedPassword;
        await _userManager.UpdateAsync(user);

        return Ok();
    }
}
