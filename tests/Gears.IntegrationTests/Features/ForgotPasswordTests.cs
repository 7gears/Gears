using FastEndpoints;
using FastEndpoints.Testing;
using Gears.Application.Features.Auth;
using Gears.IntegrationTests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Gears.IntegrationTests.Features;

public sealed class ForgotPasswordTests(InMemoryFixture f, ITestOutputHelper o) : TestClass<InMemoryFixture>(f, o)
{
    [Fact]
    public async void Test1()
    {
        var req = new ForgotPasswordRequest("root@root");
        HttpResponseMessage temp = await Fixture.Client.POSTAsync<ForgotPassword, ForgotPasswordRequest>(req);
    }

    [Fact]
    public async void Test2()
    {
        var req = new ForgotPasswordRequest("root@root");
        HttpResponseMessage temp = await Fixture.Client.POSTAsync<ForgotPassword, ForgotPasswordRequest>(req);
    }
}