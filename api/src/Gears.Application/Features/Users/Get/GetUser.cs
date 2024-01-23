namespace Gears.Application.Features.Users.Get;

public sealed class GetUser : Endpoint<GetUserRequest>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public GetUser(
        RoleManager<Role> roleManager,
        UserManager<User> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Get("api/users/{id}");
        AccessControl("Users_Get", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(GetUserRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            await SendNotFoundAsync();
            return;
        }

        var roleIds = await GetRoleIds(user, ct);

        var response = new GetUserResponse(
            user.Id,
            user.UserName,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.IsActive,
            roleIds
        );

        await SendOkAsync(response);
    }

    private async Task<IEnumerable<string>> GetRoleIds(User user, CancellationToken ct)
    {
        var roleNames = await _userManager.GetRolesAsync(user);

        return await _roleManager.Roles.AsNoTracking()
            .Where(x => roleNames.Contains(x.Name))
            .Select(x => x.Id)
            .ToListAsync(ct);
    }
}
