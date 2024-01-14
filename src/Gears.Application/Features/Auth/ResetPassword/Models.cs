namespace Gears.Application.Features.Auth.ResetPassword;

public sealed record Request
(
    string Id,
    string Token,
    string Password
);

public sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).IsNotEmpty();
        RuleFor(x => x.Token).IsNotEmpty();
        RuleFor(x => x.Password).IsNotEmpty();
    }
}
