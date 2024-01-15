using Gears.Application.Features.Auth.ResetPassword;

namespace Gears.IntegrationTests.Features.Auth.ResetPassword;

using Request = ResetPasswordRequest;
using Endpoint = Application.Features.Auth.ResetPassword.ResetPassword;

public sealed class ResetPasswordTests(TestFixture f, ITestOutputHelper o) : TestClass<TestFixture>(f, o)
{
    [Fact]
    public async Task NotFound_NotExistingUser()
    {
        var request = new Request("42", "token", "password");
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task NotFound_NotActiveUser()
    {
        var request = new Request(Fixture.NotActiveUserId, "token", "pass");
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("pass")]
    public async Task BadRequest_ActiveUser_BadPassword(string password)
    {
        var request = new Request(Fixture.ActiveUserId, "token", password);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private Task<HttpResponseMessage> Act(Request request) =>
        Fixture.Client.POSTAsync<Endpoint, Request>(request);
}
