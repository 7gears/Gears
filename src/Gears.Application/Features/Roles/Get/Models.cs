namespace Gears.Application.Features.Roles.Get;

public sealed record Request
(
    string Id
);

public sealed record Response
(
    string Id,
    string Name,
    string Description,
    bool IsDefault,
    IEnumerable<string> Permissions
);

public sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).IsNotEmpty();
    }
}
