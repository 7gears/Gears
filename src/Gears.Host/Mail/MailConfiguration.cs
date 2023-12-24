namespace Gears.Host.Mail;

public sealed class MailSettings
{
    public string From { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string DisplayName { get; set; }
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