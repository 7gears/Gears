namespace Gears.IntegrationTests.Features.Roles.Add;

public sealed class TestFixture : InMemoryFixture
{
    public RoleManager<Role> RoleManager { get; set; }

    protected override async Task SetupAsync()
    {
        this.CreateRootHttpClient();

        RoleManager = Services.GetRequiredService<RoleManager<Role>>();
        await RoleManager.CreateAsync(new Role { Name = "Existing" });
    }
}
