namespace Application.Infrastructure;

public sealed record MailRequest(
    string To,
    string Subject,
    string Body
);

public interface IMailService
{
    Task Send(MailRequest request);
}