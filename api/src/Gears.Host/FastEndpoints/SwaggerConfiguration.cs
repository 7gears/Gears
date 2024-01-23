namespace Gears.Host.FastEndpoints;

internal sealed class SwaggerSettings
{
    public bool IsEnabled { get; init; }
}

internal static class SwaggerConfiguration
{
    private const string DocumentName = "default";

    public static WebApplicationBuilder AddSwaggerServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<SwaggerSettings>(builder.Configuration.GetSection("Swagger"))
            .SwaggerDocument(x =>
            {
                x.AutoTagPathSegmentIndex = 2;
                x.ShortSchemaNames = true;
                x.DocumentSettings = s =>
                {
                    s.DocumentName = DocumentName;
                };
            });

        return builder;
    }

    public static IApplicationBuilder AddSwagger(this IApplicationBuilder builder)
    {
        var options = builder.ApplicationServices.GetRequiredService<IOptions<SwaggerSettings>>();
        if (options.Value.IsEnabled)
        {
            builder.UseSwaggerGen(uiConfig: x => x.DefaultModelsExpandDepth = -1);
        }

        return builder;
    }

    public static WebApplication AddGeneratedClientEndpoints(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<SwaggerSettings>>();
        if (options.Value.IsEnabled)
        {
            app.MapTypeScriptClientEndpoint("/ts-angular-client", DocumentName, s =>
            {
                s.ClassName = "ApiClient";
                s.InjectionTokenType = InjectionTokenType.InjectionToken;
                s.RxJsVersion = 7.8M;
                s.Template = TypeScriptTemplate.Angular;
                s.TypeScriptGeneratorSettings.Namespace = string.Empty;
                s.TypeScriptGeneratorSettings.TypeScriptVersion = 5.3M;
                s.UseSingletonProvider = true;
            });
        }

        return app;
    }
}
