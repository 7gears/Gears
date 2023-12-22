namespace Gears.Host.FastEndpoints;

internal sealed class CorsSettings
{
    public string AllowedOrigins { get; set; }
    public string AllowedMethods { get; set; }
    public string AllowedHeaders { get; set; }
    public string ExposedHeaders { get; set; }
    public bool AllowCredentials { get; set; }
    public int PreflightMaxAgeInSeconds { get; set; }
}

internal static class CorsConfiguration
{
    public static WebApplicationBuilder AddCorsServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<CorsSettings>(builder.Configuration.GetSection("Cors"))
            .AddCors();

        return builder;
    }

    public static IApplicationBuilder AddCors(this IApplicationBuilder builder)
    {
        var options = builder.ApplicationServices.GetRequiredService<IOptions<CorsSettings>>();

        builder.UseCors(CorsPolicyBuilder(options.Value));

        return builder;
    }

    private static Action<CorsPolicyBuilder> CorsPolicyBuilder(CorsSettings settings) =>
        builder =>
            {
                builder
                    .WithOrigins(settings.AllowedOrigins)
                    .WithMethods(settings.AllowedMethods)
                    .WithHeaders(settings.AllowedHeaders)
                    .WithExposedHeaders(settings.ExposedHeaders)
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(settings.PreflightMaxAgeInSeconds));

                if (!settings.AllowedOrigins.Equals("*"))
                {
                    if (settings.AllowCredentials)
                        builder.AllowCredentials();
                    else
                        builder.DisallowCredentials();
                }
            };
}