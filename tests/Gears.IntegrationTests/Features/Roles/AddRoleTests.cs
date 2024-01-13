namespace Gears.IntegrationTests.Features.Roles;

public sealed class AddRoleTests(AddRoleFixture f, ITestOutputHelper o) : TestClass<AddRoleFixture>(f, o)
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task BadRequest(string name)
    {
        var request = new AddRoleRequest(name, "description", false, new List<string>());
        var testResult = await Act(request);

        testResult.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UnprocessableEntity()
    {
        var request = new AddRoleRequest("Existing", "description", false, new List<string>());
        var testResult = await Act(request);

        testResult.Response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Create()
    {
        var request = new AddRoleRequest("NewRole", "description", false, new List<string>());
        var testResult = await Act(request);

        testResult.Response.StatusCode.Should().Be(HttpStatusCode.Created);
        testResult.Result.Id.Should().NotBeEmpty();
    }

    private Task<TestResult<AddRoleResponse>> Act(AddRoleRequest request) =>
        Fixture.Client.POSTAsync<AddRole, AddRoleRequest, AddRoleResponse>(request);
}

public sealed class AddRoleFixture : InMemoryFixture
{
    protected override async Task SetupAsync()
    {
        this.CreateRootHttpClient();

        var roleManager = Services.GetRequiredService<RoleManager<Role>>();
        await roleManager.CreateAsync(new Role { Name = "Existing" });
    }
}
