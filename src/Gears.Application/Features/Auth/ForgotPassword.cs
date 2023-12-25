namespace Gears.Application.Features.Auth;

using ForgotPasswordResultType = Results<Ok, NotFound>;

public sealed record ForgotPasswordRequest(
    string Email
);

public sealed class ForgotPasswordRequestValidator : Validator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress();
    }
}

public sealed class ForgotPassword : Endpoint<ForgotPasswordRequest, ForgotPasswordResultType>
{
    private readonly UserManager<User> _userManager;
    private readonly IMailService _mailService;
    private readonly IHttpContextService _httpContextService;

    public ForgotPassword(
        UserManager<User> userManager,
        IMailService mailService,
        IHttpContextService httpContextService)
    {
        _userManager = userManager;
        _mailService = mailService;
        _httpContextService = httpContextService;
    }

    public override void Configure()
    {
        Post("api/forgot-password");
        AllowAnonymous();
    }

    public override async Task<ForgotPasswordResultType> ExecuteAsync(ForgotPasswordRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return NotFound();
        }

        var link = await GenerateResetPasswordLink(user);

        var mailRequest = new MailRequest(user.Email, "Reset Password", link);

        await _mailService.Send(mailRequest);

        return Ok();
    }

    private async Task<string> GenerateResetPasswordLink(User user)
    {
        var origin = _httpContextService.GetOrigin();
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        UriBuilder builder = new(origin)
        {
            Path = "forgot-password"
        };
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["Email"] = user.Email;
        query["Token"] = token;
        builder.Query = query.ToString()!;

        return builder.ToString();
    }
}

public sealed class ForgotPasswordSummary : Summary<ForgotPassword>
{
    public ForgotPasswordSummary()
    {
        Response();
        Response(400);
        Response(404);
    }
}