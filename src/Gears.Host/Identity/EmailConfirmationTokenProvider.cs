namespace Gears.Host.Identity;

internal sealed class EmailConfirmationTokenProvider
(
    IDataProtectionProvider dataProtectionProvider,
    IOptions<EmailConfirmationTokenProviderOptions> options,
    ILogger<DataProtectorTokenProvider<User>> logger
) : DataProtectorTokenProvider<User>(dataProtectionProvider, options, logger);

internal sealed class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions;
