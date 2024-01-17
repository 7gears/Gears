namespace Gears.Application.Features.Users.Delete;

using Result = Results<
    Ok,
    BadRequest,
    NotFound,
    UnprocessableEntity>;

public sealed class DeleteUser
(
    UserManager<User> userManager
)
    : Endpoint<DeleteUserRequest, Result>
{
    public override void Configure()
    {
        Delete("api/users");
        AccessControl("Users_Delete", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(DeleteUserRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.UserName == Consts.Auth.RootUserUserName)
        {
            return UnprocessableEntity();
        }

        var identityResult = await userManager.DeleteAsync(user);
        if (identityResult != IdentityResult.Success)
        {
            return UnprocessableEntity();
        }

        return Ok();
    }
}
