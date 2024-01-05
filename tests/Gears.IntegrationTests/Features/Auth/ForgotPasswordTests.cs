namespace Gears.IntegrationTests.Features.Auth;

public sealed class ForgotPasswordTests(InMemoryFixture f, ITestOutputHelper o) : TestClass<InMemoryFixture>(f, o)
{
    private const string ExistingUserEmail = "root@root";
    private const string NonExistingUserEmail = "not@existing";
    private const string DeactivatedUserEmail = "not@active";

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("root")]
    public async Task ForgotPassword_BadRequest(string email)
    {
        var request = new ForgotPasswordRequest(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ForgotPassword_ExistingUser_Success()
    {
        var request = new ForgotPasswordRequest(ExistingUserEmail);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var mailService = Fixture.Services.GetRequiredService<IMailService>();
        A.CallTo(
            () => mailService.Send(A<MailRequest>.That.Matches(x => x.To == ExistingUserEmail))
        ).MustHaveHappened(1, Times.Exactly);
    }

    [Fact]
    public async Task ForgotPassword_ExistingDeactivatedUser_Success()
    {
        var userManager = Fixture.Services.GetRequiredService<UserManager<User>>();
        await userManager.CreateAsync(new User
        {
            IsActive = false,
            UserName = DeactivatedUserEmail,
            Email = DeactivatedUserEmail
        });

        var request = new ForgotPasswordRequest(DeactivatedUserEmail);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var mailService = Fixture.Services.GetRequiredService<IMailService>();
        A.CallTo(
            () => mailService.Send(A<MailRequest>.That.Matches(x => x.To == DeactivatedUserEmail))
        ).MustHaveHappened(0, Times.Exactly);
    }

    [Fact]
    public async Task ForgotPassword_NonExistingUser_Success()
    {
        var request = new ForgotPasswordRequest(NonExistingUserEmail);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var mailService = Fixture.Services.GetRequiredService<IMailService>();
        A.CallTo(
            () => mailService.Send(A<MailRequest>.That.Matches(x => x.To == NonExistingUserEmail))
        ).MustHaveHappened(0, Times.Exactly);
    }

    private Task<HttpResponseMessage> Act(ForgotPasswordRequest request) =>
        Fixture.Client.POSTAsync<ForgotPassword, ForgotPasswordRequest>(request);
}
