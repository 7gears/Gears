namespace Gears.Application.Features.Auth.ForgotPassword;

public sealed record Request
(
    string Email
);

public sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress();
    }
}
