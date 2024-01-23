using Gears.Application.Features.Auth.ForgotPassword;

namespace Gears.IntegrationTests.Features.Auth.ForgotPassword;

using Endpoint = Application.Features.Auth.ForgotPassword.ForgotPassword;
using Request = ForgotPasswordRequest;

public sealed class ForgotPasswordTests : TestClass<TestFixture>, IDisposable
{
    public ForgotPasswordTests(TestFixture f, ITestOutputHelper o) : base(f, o)
    {
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("root")]
    public async Task BadRequest(string email)
    {
        var request = new Request(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("root@root")]
    [InlineData("roOt@rOOt")]
    public async Task Success_ExistingUser(string email)
    {
        var request = new Request(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertMailService(email, 1);
    }

    [Theory]
    [InlineData("not@active")]
    [InlineData("NOT@Active")]
    public async Task Success_ExistingDeactivatedUser(string email)
    {
        var request = new Request(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertMailService(email, 0);
    }

    [Theory]
    [InlineData("not@existing")]
    [InlineData("nOt@ExIsting")]
    public async Task Success_NonExistingUser(string email)
    {
        var request = new Request(email);
        var result = await Act(request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertMailService(email, 0);
    }

    public void Dispose() =>
        FakeItEasy.Fake.ClearRecordedCalls(Fixture.MailService);

    private Task<HttpResponseMessage> Act(Request request) =>
        Fixture.Client.POSTAsync<Endpoint, Request>(request);

    private void AssertMailService(string email, int numberOfTimes)
    {
        A.CallTo(
            () => Fixture.MailService.Send(A<MailRequest>.That.Matches(x => string.Equals(x.To, email, StringComparison.OrdinalIgnoreCase)))
        ).MustHaveHappened(numberOfTimes, Times.Exactly);
    }
}
