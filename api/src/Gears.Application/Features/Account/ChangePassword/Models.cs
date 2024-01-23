namespace Gears.Application.Features.Account.ChangePassword;

public sealed record ChangePasswordRequest
(
    string Password,
    string NewPassword
);

public sealed class RequestValidator : Validator<ChangePasswordRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Password).IsNotEmpty();
        RuleFor(x => x.NewPassword).IsNotEmpty();
    }
}
