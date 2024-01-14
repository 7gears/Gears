namespace Gears.Application.Features.Roles;

using DeleteRoleResult = Results<Ok, BadRequest, NotFound, UnprocessableEntity>;

public sealed record DeleteRoleRequest(string Id);

public sealed class DeleteRoleValidator : Validator<DeleteRoleRequest>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

public sealed class DeleteRole(
    RoleManager<Role> roleManager
)
    : Endpoint<DeleteRoleRequest, DeleteRoleResult>
{
    public override void Configure()
    {
        Delete("api/roles");
        AccessControl("Roles_Delete", Apply.ToThisEndpoint);
    }

    public override async Task<DeleteRoleResult> ExecuteAsync(DeleteRoleRequest request, CancellationToken ct)
    {
        var role = await roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            return NotFound();
        }

        if (role.Name == Consts.Auth.RootRole)
        {
            return UnprocessableEntity();
        }

        var identityResult = await roleManager.DeleteAsync(role);
        if (identityResult != IdentityResult.Success)
        {
            return UnprocessableEntity();
        }

        return Ok();
    }
}
