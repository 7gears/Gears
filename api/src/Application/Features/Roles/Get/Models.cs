namespace Application.Features.Roles.Get;

public sealed record GetRoleRequest
(
    string Id
);

public sealed record GetRoleResponse
(
    string Id,
    string Name,
    string Description,
    bool IsDefault,
    IEnumerable<string> Permissions
);

public sealed class RequestValidator : Validator<GetRoleRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).IsNotEmpty();
    }
}
