namespace Gears.Application.Features.Account.GetProfile;

using Result = Results<
    Ok<GetProfileResponse>,
    NotFound>;

public sealed class GetProfile : EndpointWithoutRequest<Result>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;

    public GetProfile(
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Get("api/account/profile");
    }

    public override async Task<Result> ExecuteAsync(CancellationToken ct)
    {
        var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext!.User);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
        {
            return NotFound();
        }

        var response = new GetProfileResponse(
            user.Email,
            user.UserName,
            user.FirstName,
            user.LastName,
            user.PhoneNumber);

        return Ok(response);
    }
}
