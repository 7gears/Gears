namespace Gears.IntegrationTests.Features.Auth;

public sealed class ForgotPasswordTests(InMemoryFixture f, ITestOutputHelper o) : TestClass<InMemoryFixture>(f, o)
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("root")]
    public async Task ForgotPassword_BadRequest(string email)
    {
        var request = new ForgotPasswordRequest(email);
        var result = await Fixture.Client.POSTAsync<ForgotPassword, ForgotPasswordRequest>(request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ForgotPassword_ExistingUser_Success()
    {
        var request = new ForgotPasswordRequest("root@root");
        var result = await Fixture.Client.POSTAsync<ForgotPassword, ForgotPasswordRequest>(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var mailService = Fixture.Services.GetRequiredService<IMailService>();
        A.CallTo(
            () => mailService.Send(A<MailRequest>.That.Matches(x => x.To == "root@root"))
        ).MustHaveHappened(1, Times.Exactly);
    }

    [Fact]
    public async Task ForgotPassword_NonExistingUser_Success()
    {
        var request = new ForgotPasswordRequest("test@test");
        var result = await Fixture.Client.POSTAsync<ForgotPassword, ForgotPasswordRequest>(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var mailService = Fixture.Services.GetRequiredService<IMailService>();
        A.CallTo(
            () => mailService.Send(A<MailRequest>.That.Matches(x => x.To == "test@test"))
        ).MustHaveHappened(0, Times.Exactly);
    }
}
