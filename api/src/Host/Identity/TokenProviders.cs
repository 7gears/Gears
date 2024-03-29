﻿namespace Host.Identity;

internal sealed class PasswordResetTokenProvider : DataProtectorTokenProvider<User>
{
    public PasswordResetTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<PasswordResetTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<User>> logger) : base(dataProtectionProvider, options, logger)
    {
    }
}

internal sealed class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions
{
}

internal sealed class EmailConfirmationTokenProvider : DataProtectorTokenProvider<User>
{
    public EmailConfirmationTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<EmailConfirmationTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<User>> logger) : base(dataProtectionProvider, options, logger)
    {
    }
}

internal sealed class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
}
