namespace Gears.Application.Features.Roles.Add;

public sealed class AddRole : Endpoint<AddRoleRequest>
{
    private readonly RoleManager<Role> _roleManager;

    public AddRole(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public override void Configure()
    {
        Post("api/roles");
        AccessControl("Roles_Add", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(AddRoleRequest request, CancellationToken ct)
    {
        if (!HttpContext.User.HasPermission(Allow.Roles_ManagePermissions))
        {
            request = request with { Permissions = null };
        }

        if (!ValidatePermissions(request.Permissions))
        {
            await SendErrorsAsync();
            return;
        }

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description,
            IsDefault = request.IsDefault
        };

        var saveResult = await _roleManager.SaveRole(
            role,
            true,
            request.Permissions,
            null);

        if (!saveResult)
        {
            await SendErrorsAsync();
            return;
        }

        var response = new AddRoleResponse(role.Id);
        await SendOkAsync(response);
    }

    private static bool ValidatePermissions(IEnumerable<string> permissions) =>
        permissions == null ||
        permissions.All(Allow.AllNames().ToHashSet().Contains);
}
