namespace IntegrationTests.Features.Auth.ForgotPassword;

public sealed class TestFixture : InMemoryFixture
{
    public IMailService MailService { get; set; }

    protected override async Task SetupAsync()
    {
        await base.SetupAsync();

        MailService = Services.GetRequiredService<IMailService>();

        var userManager = Services.GetRequiredService<UserManager<User>>();
        await userManager.CreateAsync(new User
        {
            IsActive = false,
            UserName = "not@active",
            Email = "not@active"
        });
    }
}
