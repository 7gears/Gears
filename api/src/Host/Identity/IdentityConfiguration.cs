namespace Host.Identity;

internal static class IdentityConfiguration
{
    private const string EmailConfirmationTokenProviderName = nameof(EmailConfirmationTokenProviderName);
    private const string PasswordResetTokenProviderName = nameof(PasswordResetTokenProviderName);

    public static WebApplicationBuilder AddIdentityServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentityCore<User>(x =>
            {
                x.Password.RequiredLength = 6;
                x.Password.RequireDigit = false;
                x.Password.RequireLowercase = false;
                x.Password.RequireUppercase = false;
                x.Password.RequireNonAlphanumeric = false;

                x.Tokens.EmailConfirmationTokenProvider = EmailConfirmationTokenProviderName;
                x.Tokens.PasswordResetTokenProvider = PasswordResetTokenProviderName;

                x.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@/";
                x.User.RequireUniqueEmail = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddTokenProvider<EmailConfirmationTokenProvider>(EmailConfirmationTokenProviderName)
            .AddTokenProvider<PasswordResetTokenProvider>(PasswordResetTokenProviderName);

        builder.Services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromDays(3));

        builder.Services.Configure<PasswordResetTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromDays(1));

        builder.Services
            .AddAuthorization();

        return builder;
    }

    public static IApplicationBuilder AddIdentity(this IApplicationBuilder builder) =>
        builder
            .UseAuthentication()
            .UseAuthorization();
}
