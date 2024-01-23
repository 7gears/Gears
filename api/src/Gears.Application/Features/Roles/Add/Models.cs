namespace Gears.Application.Features.Roles.Add;

public sealed record AddRoleRequest
(
    string Name,
    string Description,
    bool IsDefault,
    HashSet<string> Permissions
);

public sealed record AddRoleResponse
(
    string Id
);

public sealed class RequestValidator : Validator<AddRoleRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Name).IsNotEmpty();
    }
}
