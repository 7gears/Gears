namespace Gears.Application.Features.Users.Get;

using Result = Results<
    Ok<GetUserResponse>,
    BadRequest,
    NotFound>;

public sealed class GetUser : Endpoint<GetUserRequest, Result>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public GetUser(RoleManager<Role> roleManager,
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

    public override async Task<Result> ExecuteAsync(GetUserRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            return NotFound();
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

        return Ok(response);
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
