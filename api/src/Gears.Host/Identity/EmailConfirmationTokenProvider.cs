namespace Gears.Host.Identity;

internal sealed class EmailConfirmationTokenProvider : DataProtectorTokenProvider<User>
{
    public EmailConfirmationTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<EmailConfirmationTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<User>> logger) : base(dataProtectionProvider, options, logger)
    {
    }
}

internal sealed class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions;
