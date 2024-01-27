namespace Application.Features.Users.Delete;

public sealed class DeleteUser : Endpoint<DeleteUserRequest>
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

    public override async Task HandleAsync(DeleteUserRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            await SendNotFoundAsync();
            return;
        }

        var identityResult = await _userManager.DeleteAsync(user);
        if (identityResult != IdentityResult.Success)
        {
            await SendErrorsAsync();
            return;
        }

        await SendNoContentAsync();
    }
}
