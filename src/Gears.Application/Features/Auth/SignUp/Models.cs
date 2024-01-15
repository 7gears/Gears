namespace Gears.Application.Features.Auth.SignUp;

public sealed record SignUpRequest
(
    string Email,
    string Password
);

public sealed record SignUpResponse
(
    string Id
);

public sealed class RequestValidator : Validator<SignUpRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Email)
            .IsNotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password).IsNotEmpty();
    }
}
