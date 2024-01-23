namespace Gears.IntegrationTests.Infrastructure;

internal static class Extensions
{
    public static void Remove(this IServiceCollection services, Type type)
    {
        services.Remove(services.SingleOrDefault(service => type == service.ServiceType));
    }

    public static void CreateRootHttpClient(this TestFixture<Host.Program> testFixture)
    {
        var jwtKey = testFixture.Services.GetRequiredService<IConfiguration>()["Jwt:Key"];
        var bearerToken = JWTBearer.CreateToken(
            jwtKey!,
            permissions: Allow.AllCodes());

        testFixture.Client = testFixture.CreateClient(
            x =>
            {
                x.DefaultRequestHeaders.Authorization = new("Bearer", bearerToken);
            });
    }
}
