namespace Gears.Application.Features.Users;

using DeleteUserResult = Results<Ok, BadRequest, NotFound, UnprocessableEntity>;

public sealed record DeleteUserRequest(string Id);

public sealed class DeleteUserValidator : Validator<DeleteUserRequest>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

public sealed class DeleteUser(
    UserManager<User> userManager
)
    : Endpoint<DeleteUserRequest, DeleteUserResult>
{
    public override void Configure()
    {
        Delete("api/users");
        AccessControl("Users-Delete", Apply.ToThisEndpoint);
    }

    public override async Task<DeleteUserResult> ExecuteAsync(DeleteUserRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.UserName == Consts.Auth.RootUser)
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
