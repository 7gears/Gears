namespace Host.Mail;

internal sealed class MailSettings
{
    public string From { get; init; }
    public string Host { get; init; }
    public int Port { get; init; }
    public string UserName { get; init; }
    public string Password { get; init; }
    public string DisplayName { get; init; }
}

internal static class MailConfiguration
{
    public static WebApplicationBuilder AddMailServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<MailSettings>(builder.Configuration.GetSection("Mail"));

        return builder;
    }
}
