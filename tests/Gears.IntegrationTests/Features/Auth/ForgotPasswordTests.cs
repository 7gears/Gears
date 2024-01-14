namespace Gears.IntegrationTests.Features.Auth;

public sealed class ForgotPasswordTests(
    ForgotPasswordFixture f,
    ITestOutputHelper o)
    : TestClass<ForgotPasswordFixture>(f, o), IDisposable
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("root")]
    public async Task BadRequest(string email)
    {
        var request = new ForgotPasswordRequest(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("root@root")]
    [InlineData("roOt@rOOt")]
    public async Task Success_ExistingUser(string email)
    {
        var request = new ForgotPasswordRequest(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertMailService(email, 1);
    }

    [Theory]
    [InlineData("not@active")]
    [InlineData("NOT@Active")]
    public async Task Success_ExistingDeactivatedUser(string email)
    {
        var request = new ForgotPasswordRequest(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertMailService(email, 0);
    }

    [Theory]
    [InlineData("not@existing")]
    [InlineData("nOt@ExIsting")]
    public async Task Success_NonExistingUser(string email)
    {
        var request = new ForgotPasswordRequest(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertMailService(email, 0);
    }

    public void Dispose() =>
        FakeItEasy.Fake.ClearRecordedCalls(Fixture.MailService);

    private Task<HttpResponseMessage> Act(ForgotPasswordRequest request) =>
        Fixture.Client.POSTAsync<ForgotPassword, ForgotPasswordRequest>(request);

    private void AssertMailService(string email, int numberOfTimes)
    {
        A.CallTo(
            () => Fixture.MailService.Send(A<MailRequest>.That.Matches(x => string.Equals(x.To, email, StringComparison.OrdinalIgnoreCase)))
        ).MustHaveHappened(numberOfTimes, Times.Exactly);
    }
}

public sealed class ForgotPasswordFixture : InMemoryFixture
{
    public IMailService MailService { get; set; }

    protected override async Task SetupAsync()
    {
        await base.SetupAsync();

        MailService = Services.GetRequiredService<IMailService>();

        var userManager = Services.GetRequiredService<UserManager<User>>();
        await userManager.CreateAsync(new User
        {
            IsActive = false,
            UserName = "not@active",
            Email = "not@active"
        });
    }
}
