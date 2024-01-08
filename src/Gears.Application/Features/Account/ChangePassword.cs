namespace Gears.Application.Features.Account;

using ChangePasswordResult = Results<Ok, BadRequest, NotFound>;

public sealed record ChangePasswordRequest(
    string Password,
    string NewPassword
);

public sealed class ResetPasswordRequestValidator : Validator<ChangePasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required");
    }
}

public sealed class ChangePassword(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager
)
    : Endpoint<ChangePasswordRequest, ChangePasswordResult>
{
    public override void Configure()
    {
        Post("api/account/change-password");
    }

    public override async Task<ChangePasswordResult> ExecuteAsync(ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = userManager.GetUserId(httpContextAccessor.HttpContext!.User);
        var user = await userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            return NotFound();
        }

        var result = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest();
        }

        return Ok();
    }
}
