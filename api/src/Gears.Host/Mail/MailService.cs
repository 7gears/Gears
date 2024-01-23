namespace Gears.Host.Mail;

internal sealed class MailService : IMailService
{
    private readonly MailSettings _mailSettings;

    public MailService(IOptions<MailSettings> mailOptions)
    {
        _mailSettings = mailOptions.Value;
    }

    public async Task Send(MailRequest request)
    {
        MimeMessage email = new()
        {
            Subject = request.Subject
        };
        email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
        email.To.Add(MailboxAddress.Parse(request.To));

        BodyBuilder builder = new()
        {
            HtmlBody = request.Body
        };
        email.Body = builder.ToMessageBody();

        using SmtpClient client = new();
        await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
        await client.SendAsync(email);
        await client.DisconnectAsync(true);
    }
}
