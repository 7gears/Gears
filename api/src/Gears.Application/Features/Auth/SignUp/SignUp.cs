namespace Gears.Application.Features.Auth.SignUp;

using Result = Results<
    Created<SignUpResponse>,
    BadRequest,
    UnprocessableEntity>;

public sealed class SignUp : Endpoint<SignUpRequest, Result>
{
    private readonly UserManager<User> _userManager;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IMailService _mailService;
    private readonly IHttpContextService _httpContextService;

    public SignUp(UserManager<User> userManager,
        IPasswordHasher<User> passwordHasher,
        IMailService mailService,
        IHttpContextService httpContextService)
    {
        _userManager = userManager;
        _passwordHasher = passwordHasher;
        _mailService = mailService;
        _httpContextService = httpContextService;
    }

    public override void Configure()
    {
        Post("api/auth/signup");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(SignUpRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user != null)
        {
            return UnprocessableEntity();
        }

        user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = false,
            IsActive = true,
            PasswordHash = _passwordHasher.HashPassword(null!, request.Password)
        };

        foreach (var passwordValidator in _userManager.PasswordValidators)
        {
            var passwordValidationResult = await passwordValidator.ValidateAsync(_userManager, user, request.Password);
            if (passwordValidationResult != IdentityResult.Success)
            {
                return BadRequest();
            }
        }

        var identityResult = await _userManager.CreateAsync(user);
        if (identityResult != IdentityResult.Success)
        {
            return UnprocessableEntity();
        }

        var link = await GenerateConfirmEmailLink(user);
        var mailRequest = new MailRequest(user.Email, "Confirm Email", link);

        _ = _mailService.Send(mailRequest);

        return Created(string.Empty, new SignUpResponse(user.Id));
    }

    private async Task<string> GenerateConfirmEmailLink(User user)
    {
        var origin = _httpContextService.GetOrigin();
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        UriBuilder builder = new(origin)
        {
            Path = "confirm-email"
        };
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["Id"] = user.Id;
        query["Token"] = token;
        builder.Query = query.ToString()!;

        return builder.ToString();
    }
}
