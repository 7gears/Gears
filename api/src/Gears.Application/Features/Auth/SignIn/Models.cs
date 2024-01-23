namespace Gears.Application.Features.Auth.SignIn;

public sealed record SignInRequest
(
    string Email,
    string Password
);

public sealed record SignInResponse
(
    string Token
);

public sealed class RequestValidator : Validator<SignInRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Email)
            .IsNotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
