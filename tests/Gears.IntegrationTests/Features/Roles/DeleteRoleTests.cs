
using Microsoft.AspNetCore.Identity;

namespace Gears.IntegrationTests.Features.Roles;

public sealed class DeleteRoleTests(DeleteRoleFixture f, ITestOutputHelper o) : TestClass<DeleteRoleFixture>(f, o)
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task BadRequest(string id)
    {
        var request = new DeleteRoleRequest(id);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.BadRequest, testResult.StatusCode);
    }

    [Fact]
    public async Task NotFound()
    {
        var request = new DeleteRoleRequest("1234567890");

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.NotFound, testResult.StatusCode);
    }

    [Fact]
    public async Task UnprocessableEntity_RootRole()
    {
        var request = new DeleteRoleRequest(Fixture.RootRole.Id);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, testResult.StatusCode);
    }

    [Fact]
    public async Task Success()
    {
        var request = new DeleteRoleRequest(Fixture.RegularRole.Id);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.OK, testResult.StatusCode);
        var role = Fixture.RoleManager.Roles.SingleOrDefault(x => x.Id == Fixture.RegularRole.Id);
        Assert.Null(role);
    }

    private Task<HttpResponseMessage> Act(DeleteRoleRequest request) =>
        Fixture.Client.DELETEAsync<DeleteRole, DeleteRoleRequest>(request);
}

public sealed class DeleteRoleFixture : InMemoryFixture
{
    public RoleManager<Role> RoleManager { get; set; }
    public Role RootRole { get; set; }
    public Role RegularRole { get; set; }

    protected override async Task SetupAsync()
    {
        this.CreateRootHttpClient();

        RoleManager = Services.GetRequiredService<RoleManager<Role>>();

        RegularRole = new Role { Name = "RegularRole" };

        RootRole = RoleManager.Roles.Single(x => x.Name == Consts.Auth.RootRole);
        await RoleManager.CreateAsync(RegularRole);
    }
}
