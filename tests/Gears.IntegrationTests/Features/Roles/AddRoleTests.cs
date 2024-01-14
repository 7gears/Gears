namespace Gears.IntegrationTests.Features.Roles;

public sealed class AddRoleTests(AddRoleFixture f, ITestOutputHelper o) : TestClass<AddRoleFixture>(f, o)
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task BadRequest(string name)
    {
        var request = new AddRoleRequest(
            name,
            "description",
            false,
            null);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.BadRequest, testResult.Response.StatusCode);
    }

    [Fact]
    public async Task BadRequest_WrongPermission()
    {
        var request = new AddRoleRequest(
            "NewRoleWithWrongPermission",
            "description0",
            false,
            ["NotExisting_AllowAll", "Roles_Get"]);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.BadRequest, testResult.Response.StatusCode);
    }

    [Fact]
    public async Task UnprocessableEntity()
    {
        var request = new AddRoleRequest(
            "Existing",
            "description",
            false,
            null);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, testResult.Response.StatusCode);
    }

    [Fact]
    public async Task Created_NoPermissions()
    {
        var request = new AddRoleRequest(
            "NewRoleNoPermissions",
            "desCripTion1",
            true,
            null);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.Created, testResult.Response.StatusCode);

        var role = Fixture.RoleManager.Roles.Single(x => x.Id == testResult.Result.Id);
        var claims = await Fixture.RoleManager.GetClaimsAsync(role);

        Assert.Equal("NewRoleNoPermissions", role.Name);
        Assert.Equal("desCripTion1", role.Description);
        Assert.True(role.IsDefault);
        Assert.Empty(claims);
    }

    [Fact]
    public async Task Created_WithPermissions()
    {
        var request = new AddRoleRequest(
            "NewRoleWithPermissions",
            "description2",
            false,
            ["Users_GetAll", "Roles_Get"]);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.Created, testResult.Response.StatusCode);

        var role = Fixture.RoleManager.Roles.Single(x => x.Id == testResult.Result.Id);
        var claims = await Fixture.RoleManager.GetClaimsAsync(role);

        Assert.Equal("NewRoleWithPermissions", role.Name);
        Assert.Equal("description2", role.Description);
        Assert.False(role.IsDefault);
        Assert.Equal(2, claims.Count);
    }

    private Task<TestResult<AddRoleResponse>> Act(AddRoleRequest request) =>
        Fixture.Client.POSTAsync<AddRole, AddRoleRequest, AddRoleResponse>(request);
}

public sealed class AddRoleFixture : InMemoryFixture
{
    public RoleManager<Role> RoleManager { get; set; }

    protected override async Task SetupAsync()
    {
        this.CreateRootHttpClient();

        RoleManager = Services.GetRequiredService<RoleManager<Role>>();
        await RoleManager.CreateAsync(new Role { Name = "Existing" });
    }
}
