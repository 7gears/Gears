namespace Application.Features.Users.Update;

public sealed class UpdateUser : Endpoint<UpdateUserRequest>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public UpdateUser(
        RoleManager<Role> roleManager,
        UserManager<User> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Put("api/users/{id}");
        AccessControl("Users_Update", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(UpdateUserRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            await SendNotFoundAsync();
            return;
        }

        var rolesToAdd = Enumerable.Empty<string>();
        var rolesToDelete = Enumerable.Empty<string>();

        if (HttpContext.User.HasPermission(Allow.Users_ManageRoles))
        {
            var roleInfos = await GetRoleInfos(request, user, ct);

            if (!UpdateUserRequestRoleParser.TryParseRoles(roleInfos, out rolesToAdd, out rolesToDelete))
            {
                await SendErrorsAsync();
                return;
            }
        }

        user.Email = request.Email;
        user.UserName = request.UserName;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.IsActive = request.IsActive;

        var saveResult = await _userManager.SaveUser(
            user,
            false,
            rolesToAdd,
            rolesToDelete);
        if (!saveResult)
        {
            await SendErrorsAsync();
            return;
        }

        await SendNoContentAsync();
    }

    private async Task<RoleInfos> GetRoleInfos(
        UpdateUserRequest request,
        User user,
        CancellationToken ct)
    {
        var allRoles = await _roleManager.Roles.AsNoTracking()
            .ToListAsync(ct);

        var userRoleNames = await _userManager.GetRolesAsync(user);

        return new(request.RoleIds ?? new HashSet<string>(), allRoles, userRoleNames);
    }
}

internal sealed record RoleInfos(
    HashSet<string> RequestRoleIds,
    List<Role> AllRoles,
    IList<string> UserRoleNames
);
