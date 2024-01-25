namespace Application.Features.Account.GetProfile;

public sealed class GetProfile : EndpointWithoutRequest
{
    private readonly UserManager<User> _userManager;

    public GetProfile(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Get("api/account/profile");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = _userManager.GetUserId(HttpContext.User);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
        {
            await SendNotFoundAsync();
            return;
        }

        var response = new GetProfileResponse(
            user.Email,
            user.UserName,
            user.FirstName,
            user.LastName,
            user.PhoneNumber);

        await SendOkAsync(response);
    }
}
