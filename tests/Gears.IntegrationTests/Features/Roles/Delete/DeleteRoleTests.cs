using Gears.Application.Features.Roles.Delete;

namespace Gears.IntegrationTests.Features.Roles.Delete;

using Endpoint = DeleteRole;
using Request = DeleteRoleRequest;

public sealed class DeleteRoleTests(TestFixture f, ITestOutputHelper o) : TestClass<TestFixture>(f, o)
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task BadRequest(string id)
    {
        var request = new Request(id);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.BadRequest, testResult.StatusCode);
    }

    [Fact]
    public async Task NotFound()
    {
        var request = new Request("1234567890");

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.NotFound, testResult.StatusCode);
    }

    [Fact]
    public async Task UnprocessableEntity_RootRole()
    {
        var request = new Request(Fixture.RootRole.Id);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, testResult.StatusCode);
    }

    [Fact]
    public async Task Success()
    {
        var request = new Request(Fixture.RegularRole.Id);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.OK, testResult.StatusCode);
        var role = Fixture.RoleManager.Roles.SingleOrDefault(x => x.Id == Fixture.RegularRole.Id);
        Assert.Null(role);
    }

    private Task<HttpResponseMessage> Act(Request deleteRoleRequest) =>
        Fixture.Client.DELETEAsync<Endpoint, Request>(deleteRoleRequest);
}
