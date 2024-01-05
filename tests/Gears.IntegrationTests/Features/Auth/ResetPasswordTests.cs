namespace Gears.IntegrationTests.Features.Auth;

public sealed class ResetPasswordTests(ResetPasswordFixture f, ITestOutputHelper o) : TestClass<ResetPasswordFixture>(f, o)
{
    [Fact]
    public async Task NotExistingUser_NotFound()
    {
        var request = new ResetPasswordRequest("42", "token", "password");
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task NotActiveUser_NotFound()
    {
        var request = new ResetPasswordRequest(Fixture.NotActiveUserId, "token", "pass");
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("pass")]
    public async Task ActiveUser_BadPassword_BadRequest(string password)
    {
        var request = new ResetPasswordRequest(Fixture.ActiveUserId, "token", password);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private Task<HttpResponseMessage> Act(ResetPasswordRequest request) =>
        Fixture.Client.POSTAsync<ResetPassword, ResetPasswordRequest>(request);
}

public sealed class ResetPasswordFixture : InMemoryFixture
{
    public string ActiveUserId { get; set; }
    public string NotActiveUserId { get; set; }

    protected override async Task SetupAsync()
    {
        await base.SetupAsync();

        var userManager = Services.GetRequiredService<UserManager<User>>();
        var activeUser = new User
        {
            IsActive = true,
            UserName = "active@active",
            Email = "active@active"
        };
        var notActiveUser = new User
        {
            IsActive = false,
            UserName = "not@active",
            Email = "not@active"
        };
        await userManager.CreateAsync(activeUser);
        await userManager.CreateAsync(notActiveUser);

        ActiveUserId = activeUser.Id;
        NotActiveUserId = notActiveUser.Id;
    }
}
