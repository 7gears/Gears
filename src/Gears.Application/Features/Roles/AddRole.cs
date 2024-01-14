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
        AccessControl("Roles_Add", Apply.ToThisEndpoint);
    }

    public override async Task<AddRoleResult> ExecuteAsync(AddRoleRequest request, CancellationToken ct)
    {
        if (!ValidatePermissions(request.Permissions))
        {
            return BadRequest();
        }

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description,
            IsDefault = request.IsDefault
        };

        var result = await roleManager.SaveRoleWithPermissions(
            role,
            true,
            request.Permissions);

        if (!result)
        {
            return UnprocessableEntity();
        }

        return Created(string.Empty, new AddRoleResponse(role.Id));
    }

    private static bool ValidatePermissions(IEnumerable<string> permissions) =>
        permissions == null || permissions.All(Allow.AllNames().ToHashSet().Contains);
}
