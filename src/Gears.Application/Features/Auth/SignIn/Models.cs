namespace Gears.Application.Features.Auth.SignIn;

public sealed record Request
(
    string Email,
    string Password
);

public sealed record Response
(
    string Token
);

public sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
