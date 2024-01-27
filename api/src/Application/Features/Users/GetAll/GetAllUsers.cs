namespace Application.Features.Users.GetAll;

public sealed class GetAllUsers : EndpointWithoutRequest<List<GetAllUsersResponse>>
{
    private readonly UserManager<User> _userManager;

    public GetAllUsers(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Get("api/users");
        AccessControl("Users_GetAll", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await _userManager.Users.AsNoTracking()
            .Select(x => new GetAllUsersResponse(
                x.Id,
                x.UserName,
                x.Email,
                x.FirstName,
                x.LastName,
                x.PhoneNumber,
                x.IsActive))
            .ToListAsync(ct);

        await SendAsync(response);
    }
}
