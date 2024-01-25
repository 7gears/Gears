namespace Gears.IntegrationTests.Features.Roles.Delete;

public sealed class TestFixture : InMemoryFixture
{
    public RoleManager<Role> RoleManager { get; set; }
    public Role RootRole { get; set; }
    public Role RegularRole { get; set; }

    protected override async Task SetupAsync()
    {
        this.CreateRootHttpClient();

        RoleManager = Services.GetRequiredService<RoleManager<Role>>();

        RegularRole = new Role { Name = "RegularRole" };

        RootRole = RoleManager.Roles.Single(x => x.Name == Consts.Auth.RootRoleName);
        await RoleManager.CreateAsync(RegularRole);
    }
}
