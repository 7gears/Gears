using Application.Features.Roles.Delete;

namespace IntegrationTests.Features.Roles.Delete;

using Endpoint = DeleteRole;
using Request = DeleteRoleRequest;

public sealed class DeleteRoleTests : TestClass<TestFixture>
{
    public DeleteRoleTests(TestFixture f, ITestOutputHelper o) : base(f, o)
    {
    }

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
    public async Task NoContent()
    {
        var request = new Request(Fixture.RegularRole.Id);

        var testResult = await Act(request);

        Assert.Equal(HttpStatusCode.NoContent, testResult.StatusCode);
        var role = Fixture.RoleManager.Roles.SingleOrDefault(x => x.Id == Fixture.RegularRole.Id);
        Assert.Null(role);
    }

    private Task<HttpResponseMessage> Act(Request deleteRoleRequest) =>
        Fixture.Client.DELETEAsync<Endpoint, Request>(deleteRoleRequest);
}
