namespace Gears.Application.Features.Users.Add;

using Result = Results<
    Created<AddUserResponse>,
    BadRequest,
    UnprocessableEntity>;

public sealed class AddUser : Endpoint<AddUserRequest, Result>
{
    private readonly IOptions<PasswordOptions> _passwordOptions;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AddUser(
        IOptions<PasswordOptions> passwordOptions,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IPasswordHasher<User> passwordHasher)
    {
        _passwordOptions = passwordOptions;
        _userManager = userManager;
        _roleManager = roleManager;
        _passwordHasher = passwordHasher;
    }

    public override void Configure()
    {
        Post("api/users");
        AccessControl("Users_Add", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(AddUserRequest request, CancellationToken ct)
    {
        var rolesToAdd = Enumerable.Empty<string>();

        if (HttpContext.User.HasPermission(Allow.Users_ManageRoles))
        {
            var roleInfos = await GetRoleInfos(request, ct);
            if (!AddUserRequestRoleParser.TryParseRoles(roleInfos, out rolesToAdd))
            {
                return BadRequest();
            }
        }

        var password = Utils.GenerateRandomPassword(_passwordOptions.Value);
        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName != string.Empty ? request.UserName : request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.IsActive,
            EmailConfirmed = true,
            PasswordHash = _passwordHasher.HashPassword(null!, password)
        };

        var result = await _userManager.SaveUser(
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
        var allRoles = await _roleManager.Roles.AsNoTracking()
            .ToListAsync(ct);

        return new(request.RoleIds ?? new HashSet<string>(), allRoles);
    }
}

internal sealed record RoleInfos(
    HashSet<string> RequestRoleIds,
    List<Role> AllRoles
);
