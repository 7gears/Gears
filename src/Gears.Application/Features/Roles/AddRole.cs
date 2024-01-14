namespace Gears.Application.Features.Roles;

using AddRoleResult = Results<Created<AddRoleResponse>, BadRequest, UnprocessableEntity>;

public sealed record AddRoleRequest(
    string Name,
    string Description,
    bool IsDefault,
    HashSet<string> Permissions);

public sealed record AddRoleResponse(
    string Id
);

public sealed class AddRoleValidator : Validator<AddRoleRequest>
{
    public AddRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}

public sealed class AddRole(
    RoleManager<Role> roleManager
)
    : Endpoint<AddRoleRequest, AddRoleResult>
{
    public override void Configure()
    {
        Post("api/roles");
        AccessControl("Roles-Add", Apply.ToThisEndpoint);
    }

    public override async Task<AddRoleResult> ExecuteAsync(AddRoleRequest request, CancellationToken ct)
    {
        var role = new Role
        {
            Name = request.Name,
            Description = request.Description,
            IsDefault = request.IsDefault
        };

        var allPermissions = Allow.AllNames().ToHashSet();

        var claims = Enumerable.Empty<Claim>();
        if (request.Permissions != null)
        {
            claims = request.Permissions
                .Where(allPermissions.Contains)
                .Select(x => new Claim(Consts.Auth.PermissionClaimType, x))
                .ToHashSet();
        }

        var result = await Save(role, claims);
        if (!result)
        {
            return UnprocessableEntity();
        }

        return Created(string.Empty, new AddRoleResponse(role.Id));
    }

    private async Task<bool> Save(Role role, IEnumerable<Claim> claims)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            var result = await roleManager.CreateAsync(role);
            if (result != IdentityResult.Success)
            {
                return false;
            }

            foreach (var claim in claims)
            {
                result = await roleManager.AddClaimAsync(role, claim);
                if (result != IdentityResult.Success)
                {
                    return false;
                }
            }

            scope.Complete();
            return true;
        }
        catch
        {
            return false;
        }
    }

}
