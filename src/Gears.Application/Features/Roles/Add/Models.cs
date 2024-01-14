namespace Gears.Application.Features.Roles.Add;

public sealed record Request
(
    string Name,
    string Description,
    bool IsDefault,
    HashSet<string> Permissions
);

public sealed record Response(string Id);

public sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}
