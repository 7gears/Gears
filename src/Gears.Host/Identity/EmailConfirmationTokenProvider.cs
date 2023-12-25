using Microsoft.AspNetCore.DataProtection;

namespace Gears.Host.Identity;

public sealed class EmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
{
    public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
        IOptions<EmailConfirmationTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}
public sealed class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
}