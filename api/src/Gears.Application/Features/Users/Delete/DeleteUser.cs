namespace Gears.Application.Features.Users.Delete;

using Result = Results<
    Ok,
    BadRequest,
    NotFound,
    UnprocessableEntity>;

public sealed class DeleteUser : Endpoint<DeleteUserRequest, Result>
{
    private readonly UserManager<User> _userManager;

    public DeleteUser(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Delete("api/users/{id}");
        AccessControl("Users_Delete", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(DeleteUserRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.UserName == Consts.Auth.RootUserUserName)
        {
            return UnprocessableEntity();
        }

        var identityResult = await _userManager.DeleteAsync(user);
        if (identityResult != IdentityResult.Success)
        {
            return UnprocessableEntity();
        }

        return Ok();
    }
}
