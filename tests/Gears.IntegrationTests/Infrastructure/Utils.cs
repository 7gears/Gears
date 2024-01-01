namespace Gears.IntegrationTests.Infrastructure;

internal static class Utils
{
    public static void Remove(this IServiceCollection services, Type type)
    {
        services.Remove(services.SingleOrDefault(service => type == service.ServiceType));
    }
}
