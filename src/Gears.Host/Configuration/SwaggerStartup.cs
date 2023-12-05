using Microsoft.Extensions.Options;

namespace Gears.Host.Configuration;

internal sealed class SwaggerSettings
{
    public bool IsEnabled { get; init; }
}

internal static class SwaggerStartup
{
    public static WebApplicationBuilder ConfigureSwaggerSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<SwaggerSettings>(builder.Configuration.GetSection("Swagger"));

        return builder;
    }

    public static WebApplicationBuilder ConfigureSwaggerServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        return builder;
    }

    public static IApplicationBuilder ConfigureSwaggerMiddleware(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<SwaggerSettings>>();
        if (options.Value.IsEnabled)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}