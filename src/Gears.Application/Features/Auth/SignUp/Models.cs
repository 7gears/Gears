namespace Gears.Application.Features.Auth.SignUp;

public sealed record Request
(
    string Email,
    string Password
);

public sealed record Response
(
    string Id
);

public sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Email)
            .IsNotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password).IsNotEmpty();
    }
}
