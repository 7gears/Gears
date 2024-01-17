namespace Gears.Application.Features.Users.Update;

using Result = Results<
    NoContent,
    BadRequest,
    NotFound,
    UnprocessableEntity>;

public sealed class UpdateUser
(
    RoleManager<Role> roleManager,
    UserManager<User> userManager,
    IHttpContextService httpContextService
)
    : Endpoint<UpdateUserRequest, Result>
{
    public override void Configure()
    {
        Put("api/users/{id}");
        AccessControl("Users_Update", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(UpdateUserRequest request, CancellationToken ct)
    {
        if (!httpContextService.HasPermission(Allow.Users_ManageRoles))
        {
            request = request with { RoleIds = null };
        }
        else if (request.RoleIds == null)
        {
            request = request with { RoleIds = [] };
        }

        var user = await userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.UserName == Consts.Auth.RootUserUserName)
        {
            return UnprocessableEntity();
        }

        var allRoles = await roleManager.Roles.AsNoTracking()
            .ToListAsync(ct);
        var userRoles = await userManager.GetRolesAsync(user);

        if (!UserRoleProcessor.TryParseRoles(
                request,
                allRoles,
                userRoles,
                out var rolesToAdd,
                out var rolesToDelete))
        {
            return BadRequest();
        }

        user.Email = request.Email;
        user.UserName = request.UserName;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.IsActive = request.IsActive;

        var saveResult = await userManager.SaveUser(
            user,
            false,
            rolesToAdd,
            rolesToDelete);
        if (!saveResult)
        {
            return UnprocessableEntity();
        }

        return NoContent();
    }
}
