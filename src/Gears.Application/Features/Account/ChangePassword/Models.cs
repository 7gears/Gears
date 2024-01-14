namespace Gears.Application.Features.Account.ChangePassword;

public sealed record Request
(
    string Password,
    string NewPassword
);

public sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Password).IsNotEmpty();
        RuleFor(x => x.NewPassword).IsNotEmpty();
    }
}
