namespace Gears.IntegrationTests.Features.Auth;

public sealed class ForgotPasswordTests : TestClass<ForgotPasswordTestsFixture>, IDisposable
{
    private readonly IMailService _mailService;

    public ForgotPasswordTests(ForgotPasswordTestsFixture f, ITestOutputHelper o) : base(f, o)
    {
        _mailService = Fixture.Services.GetRequiredService<IMailService>();
    }

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

    [Theory]
    [InlineData("root@root")]
    [InlineData("roOt@rOOt")]
    public async Task ForgotPassword_ExistingUser_Success(string email)
    {
        var request = new ForgotPasswordRequest(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertMailService(email, 1);
    }

    [Theory]
    [InlineData("not@active")]
    [InlineData("NOT@Active")]
    public async Task ForgotPassword_ExistingDeactivatedUser_Success(string email)
    {
        var request = new ForgotPasswordRequest(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertMailService(email, 0);
    }

    [Theory]
    [InlineData("not@existing")]
    [InlineData("nOt@ExIsting")]
    public async Task ForgotPassword_NonExistingUser_Success(string email)
    {
        var request = new ForgotPasswordRequest(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertMailService(email, 0);
    }

    public void Dispose() =>
        FakeItEasy.Fake.ClearRecordedCalls(_mailService);

    private Task<HttpResponseMessage> Act(ForgotPasswordRequest request) =>
        Fixture.Client.POSTAsync<ForgotPassword, ForgotPasswordRequest>(request);

    private void AssertMailService(string email, int numberOfTimes)
    {
        A.CallTo(
            () => _mailService.Send(A<MailRequest>.That.Matches(x => string.Equals(x.To, email, StringComparison.OrdinalIgnoreCase)))
        ).MustHaveHappened(numberOfTimes, Times.Exactly);
    }
}

public sealed class ForgotPasswordTestsFixture : InMemoryFixture
{
    protected override async Task SetupAsync()
    {
        await base.SetupAsync();

        var userManager = Services.GetRequiredService<UserManager<User>>();
        await userManager.CreateAsync(new User
        {
            IsActive = false,
            UserName = "not@active",
            Email = "not@active"
        });
    }
}

