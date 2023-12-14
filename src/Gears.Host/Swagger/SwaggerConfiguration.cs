using FastEndpoints.Swagger;

namespace Gears.Host.Swagger;

internal sealed class SwaggerSettings
{
    public bool IsEnabled { get; init; }
}

internal static class SwaggerConfiguration
{
    public static WebApplicationBuilder ConfigureSwaggerSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<SwaggerSettings>(builder.Configuration.GetSection("Swagger"));

        return builder;
    }

    public static IServiceCollection ConfigureSwaggerServices(this IServiceCollection services)
    {
        services.SwaggerDocument();

        return services;
    }

    public static IApplicationBuilder ConfigureSwaggerMiddleware(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<SwaggerSettings>>();
        if (options.Value.IsEnabled)
        {
            app.UseSwaggerGen();
        }

        return app;
    }
}