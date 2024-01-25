namespace Application.Features.Roles.Update;

public sealed record UpdateRoleRequest
(
    string Id,
    string Name,
    string Description,
    bool IsDefault,
    HashSet<string> Permissions
);

public sealed class RequestValidator : Validator<UpdateRoleRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).IsNotEmpty();
        RuleFor(x => x.Name).IsNotEmpty();
    }
}
