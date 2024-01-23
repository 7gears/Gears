namespace Gears.Application.Features.Auth.ConfirmEmail;

public sealed record ConfirmEmailRequest
(
    string Id,
    string Token
);

public sealed class RequestValidator : Validator<ConfirmEmailRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).IsNotEmpty();
        RuleFor(x => x.Token).IsNotEmpty();
    }
}
