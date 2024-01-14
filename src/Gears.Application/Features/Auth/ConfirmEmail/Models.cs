namespace Gears.Application.Features.Auth.ConfirmEmail;

public sealed record Request
(
    string Id,
    string Token
);

public sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).IsNotEmpty();
        RuleFor(x => x.Token).IsNotEmpty();
    }
}
