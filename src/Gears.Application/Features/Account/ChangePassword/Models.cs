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
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required");
    }
}
