namespace Gears.Host.FastEndpoints;

internal sealed class SwaggerSettings
{
    public bool IsEnabled { get; init; }
}

internal static class FastEndpointsConfiguration
{
    private const string DocumentName = "default";

    public static WebApplicationBuilder ConfigureFastEndpointsServices(this WebApplicationBuilder builder)
    {
        var key = builder.Configuration.GetValue<string>($"Jwt:{nameof(JwtConfiguration.Key)}");

        builder.Services
            .Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"))
            .Configure<SwaggerSettings>(builder.Configuration.GetSection("Swagger"))
            .AddFastEndpoints(x =>
            {
                
                x.DisableAutoDiscovery = true;
                x.Assemblies = new[] { typeof(Application.Features.Users.GetAllUsersEndpoint).Assembly };
            })
            .AddJWTBearerAuth(key)
            .SwaggerDocument(x =>
            {
                x.ShortSchemaNames = true;
                x.DocumentSettings = s =>
                {
                    s.DocumentName = DocumentName;
                };
            })
            .RegisterServicesFromGearsHost();

        return builder;
    }

    public static IApplicationBuilder ConfigureFastEndpointsMiddleware(this IApplicationBuilder builder)
    {
        builder.UseFastEndpoints(x =>
        {
            x.Endpoints.ShortNames = true;
            x.Errors.ProducesMetadataType = typeof(ProblemDetails);
            x.Errors.UseProblemDetails();
        });

        var options = builder.ApplicationServices.GetRequiredService<IOptions<SwaggerSettings>>();
        if (options.Value.IsEnabled)
        {
            builder.UseSwaggerGen();
        }

        return builder;
    }

    public static WebApplication ConfigureGeneratedClientEndpoints(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<SwaggerSettings>>();
        if (options.Value.IsEnabled)
        {
            app.MapCSharpClientEndpoint("/cs-client", DocumentName, s =>
            {
                s.ClassName = "ApiClient";
                s.CSharpGeneratorSettings.Namespace = "Gears";
            });

            app.MapTypeScriptClientEndpoint("/ts-client", DocumentName, s =>
            {
                s.ClassName = "ApiClient";
                s.TypeScriptGeneratorSettings.Namespace = "Gears";
            });
        }

        return app;
    }
}