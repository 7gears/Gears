namespace Gears.Application.Features.Users.Add;

using Result = Results<
    Created<AddUserResponse>,
    BadRequest,
    UnprocessableEntity>;

public sealed class AddUser
(
    IOptions<PasswordOptions> passwordOptions,
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IPasswordHasher<User> passwordHasher,
    IHttpContextService httpContextService
)
    : Endpoint<AddUserRequest, Result>
{
    public override void Configure()
    {
        Post("api/users");
        AccessControl("Users_Add", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(AddUserRequest request, CancellationToken ct)
    {
        var rolesToAdd = Enumerable.Empty<string>();

        if (httpContextService.HasPermission(Allow.Users_ManageRoles))
        {
            var roleInfos = await GetRoleInfos(request, ct);
            if (!AddUserRequestRoleParser.TryParseRoles(roleInfos, out rolesToAdd))
            {
                return BadRequest();
            }
        }

        var password = Utils.GenerateRandomPassword(passwordOptions.Value);
        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName != string.Empty ? request.UserName : request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.IsActive,
            EmailConfirmed = true,
            PasswordHash = passwordHasher.HashPassword(null!, password)
        };

        var result = await userManager.SaveUser(
            user,
            true,
            rolesToAdd,
            null);

        if (!result)
        {
            return UnprocessableEntity();
        }

        return Created(string.Empty, new AddUserResponse(user.Id));
    }

    private async Task<RoleInfos> GetRoleInfos(AddUserRequest request, CancellationToken ct)
    {
        var allRoles = await roleManager.Roles.AsNoTracking()
            .ToListAsync(ct);

        return new(request.RoleIds ?? [], allRoles);
    }
}

internal sealed record RoleInfos(
    HashSet<string> RequestRoleIds,
    List<Role> AllRoles
);
