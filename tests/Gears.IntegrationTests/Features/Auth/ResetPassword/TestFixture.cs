namespace Gears.IntegrationTests.Features.Auth.ResetPassword;

public sealed class TestFixture : InMemoryFixture
{
    public string ActiveUserId { get; set; }
    public string NotActiveUserId { get; set; }

    protected override async Task SetupAsync()
    {
        await base.SetupAsync();

        var userManager = Services.GetRequiredService<UserManager<User>>();
        var activeUser = new User
        {
            IsActive = true,
            UserName = "active@active",
            Email = "active@active"
        };
        var notActiveUser = new User
        {
            IsActive = false,
            UserName = "not@active",
            Email = "not@active"
        };
        await userManager.CreateAsync(activeUser);
        await userManager.CreateAsync(notActiveUser);

        ActiveUserId = activeUser.Id;
        NotActiveUserId = notActiveUser.Id;
    }
}
