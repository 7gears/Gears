namespace Gears.Application.Features.Roles;

using GetRoleResult = Results<Ok<GetRoleResponse>, BadRequest, NotFound>;

public sealed record GetRoleRequest(
    string Id
);

public sealed record GetRoleResponse(
    string Id,
    string Name,
    string Description,
    bool IsDefault,
    IEnumerable<string> Permissions
);

public sealed class GetRoleRequestValidator : Validator<GetRoleRequest>
{
    public GetRoleRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

public sealed class GetRole(
    RoleManager<Role> roleManager
)
    : Endpoint<GetRoleRequest, GetRoleResult>
{
    public override void Configure()
    {
        Get("api/roles/{id}");
        AccessControl("Roles_Get", Apply.ToThisEndpoint);
    }

    public override async Task<GetRoleResult> ExecuteAsync(GetRoleRequest request, CancellationToken ct)
    {
        var role = await roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            return NotFound();
        }

        var claims = await roleManager.GetClaimsAsync(role);
        var permissions = claims
            .Where(x => x.Type == Consts.Auth.PermissionClaimType)
            .Select(x => x.Value);

        var result = new GetRoleResponse(
            role.Id,
            role.Name,
            role.Description,
            role.IsDefault,
            permissions
        );

        return Ok(result);
    }
}
