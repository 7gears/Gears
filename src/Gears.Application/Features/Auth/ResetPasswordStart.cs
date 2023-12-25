namespace Gears.Application.Features.Auth;

using ResetPasswordStartResultType = Results<Ok, NotFound>;

public sealed record ResetPasswordStartRequest(
    string Email
);

public sealed class ResetPasswordStartRequestValidator : Validator<ResetPasswordStartRequest>
{
    public ResetPasswordStartRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress();
    }
}

public sealed class ResetPasswordStart : Endpoint<ResetPasswordStartRequest, ResetPasswordStartResultType>
{
    private readonly UserManager<User> _userManager;
    private readonly IMailService _mailService;
    private readonly IHttpContextService _httpContextService;

    public ResetPasswordStart(
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
        Post("api/reset-password-start");
        AllowAnonymous();
    }

    public override async Task<ResetPasswordStartResultType> ExecuteAsync(ResetPasswordStartRequest request, CancellationToken ct)
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
            Path = "reset-password-start"
        };
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["Email"] = user.Email;
        query["Token"] = token;
        builder.Query = query.ToString()!;

        return builder.ToString();
    }
}

public sealed class ResetPasswordStartSummary : Summary<ResetPasswordStart>
{
    public ResetPasswordStartSummary()
    {
        Response();
        Response(400);
        Response(404);
    }
}