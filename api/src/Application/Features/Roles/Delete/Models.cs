namespace Application.Features.Roles.Delete;

public sealed record DeleteRoleRequest
(
    string Id
);

public sealed class RequestValidator : Validator<DeleteRoleRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).IsNotEmpty();
    }
}
