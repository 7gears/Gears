namespace Gears.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPassword : Endpoint<ForgotPasswordRequest, Ok>
{
    private readonly UserManager<User> _userManager;
    private readonly IMailService _mailService;
    private readonly IHttpContextService _httpContextService;

    public ForgotPassword(UserManager<User> userManager,
        IMailService mailService,
        IHttpContextService httpContextService)
    {
        _userManager = userManager;
        _mailService = mailService;
        _httpContextService = httpContextService;
    }

    public override void Configure()
    {
        Post("api/auth/forgot-password");
        AllowAnonymous();
    }

    public override async Task<Ok> ExecuteAsync(ForgotPasswordRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is not { IsActive: true })
        {
            // Return a consistent response for both existent and non-existent accounts
            return Ok();
        }

        var link = await GenerateResetPasswordLink(user);

        var mailRequest = new MailRequest(user.Email, "Reset Password", link);

        _ = _mailService.Send(mailRequest);

        return Ok();
    }

    private async Task<string> GenerateResetPasswordLink(User user)
    {
        var origin = _httpContextService.GetOrigin();
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        UriBuilder builder = new(origin) { Path = "reset-password" };
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["Id"] = user.Id;
        query["Token"] = token;
        builder.Query = query.ToString()!;

        return builder.ToString();
    }
}
