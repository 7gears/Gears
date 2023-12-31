namespace Gears.IntegrationTests.Features;

public sealed class ForgotPasswordTests(InMemoryFixture f, ITestOutputHelper o) : TestClass<InMemoryFixture>(f, o)
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("root")]
    public async void ForgotPassword_BadRequest(string email)
    {
        var request = new ForgotPasswordRequest(email);
        var result = await Fixture.Client.POSTAsync<ForgotPassword, ForgotPasswordRequest>(request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async void ForgotPassword_ExistingUser_Success()
    {
        var request = new ForgotPasswordRequest("root@root");
        var result = await Fixture.Client.POSTAsync<ForgotPassword, ForgotPasswordRequest>(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async void ForgotPassword_NonExistingUser_Success()
    {
        var request = new ForgotPasswordRequest("test@test");
        var result = await Fixture.Client.POSTAsync<ForgotPassword, ForgotPasswordRequest>(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}