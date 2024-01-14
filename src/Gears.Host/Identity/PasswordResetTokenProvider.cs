namespace Gears.Host.Identity;

internal sealed class PasswordResetTokenProvider
(
    IDataProtectionProvider dataProtectionProvider,
    IOptions<PasswordResetTokenProviderOptions> options,
    ILogger<DataProtectorTokenProvider<User>> logger
) : DataProtectorTokenProvider<User>(dataProtectionProvider, options, logger);

internal sealed class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions;
