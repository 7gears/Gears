namespace Gears.Host.Swagger;

internal sealed class SwaggerSettings
{
    public bool IsEnabled { get; init; }
}

internal static class SwaggerConfiguration
{
    public static WebApplicationBuilder ConfigureSwaggerServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<SwaggerSettings>(builder.Configuration.GetSection("Swagger"))
            .SwaggerDocument();

        return builder;
    }

    public static IApplicationBuilder ConfigureSwaggerMiddleware(this IApplicationBuilder builder)
    {
        var options = builder.ApplicationServices.GetRequiredService<IOptions<SwaggerSettings>>();
        if (options.Value.IsEnabled)
        {
            builder.UseSwaggerGen();
        }

        return builder;
    }
}