namespace Application.Features.Auth.ResetPassword;

public sealed record ResetPasswordRequest
(
    string Id,
    string Token,
    string Password
);

public sealed class RequestValidator : Validator<ResetPasswordRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).IsNotEmpty();
        RuleFor(x => x.Token).IsNotEmpty();
        RuleFor(x => x.Password).IsNotEmpty();
    }
}
