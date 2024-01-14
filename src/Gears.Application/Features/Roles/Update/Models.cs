namespace Gears.Application.Features.Roles.Update;

public sealed record Request
(
    string Id,
    string Name,
    string Description,
    bool IsDefault,
    HashSet<string> Permissions
);

public sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}
