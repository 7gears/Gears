namespace Gears.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordRequest
(
    string Email
);

public sealed class RequestValidator : Validator<ForgotPasswordRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Email)
            .IsNotEmpty()
            .EmailAddress();
    }
}
