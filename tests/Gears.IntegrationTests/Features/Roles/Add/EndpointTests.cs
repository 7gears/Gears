using Gears.Application.Features.Roles.Add;

namespace Gears.IntegrationTests.Features.Roles.Add;

public sealed class EndpointTests(TestFixture f, ITestOutputHelper o) : TestClass<TestFixture>(f, o)
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task BadRequest(string name)
    {
        var request = new Request(
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
        var request = new Request(
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
        var request = new Request(
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
        var request = new Request(
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
        var request = new Request(
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

    private Task<TestResult<Response>> Act(Request request) =>
        Fixture.Client.POSTAsync<Endpoint, Request, Response>(request);
}
