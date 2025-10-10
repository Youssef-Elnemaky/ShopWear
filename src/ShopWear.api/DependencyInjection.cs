using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using ShopWear.api.OpenApi.Transformers;
using ShopWear.Application;
using ShopWear.Application.Services.Email;

namespace ShopWear.api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        // API level
        services.AddCustomApiVersioning()
                .AddApiDocumentation()
                .AddScoped<IEmailService, MailkitSmtpEmailSender>()
                .Configure<SmtpOptions>(configuration.GetSection("Smtp"));


        // register application services (it internally registers data access services as well)
        services.AddApplication(configuration);

        return services;
    }

    private static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    private static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        string[] versions = ["v1", "v2"];

        foreach (var version in versions)
        {
            services.AddOpenApi(version, options =>
            {
                options.AddDocumentTransformer<VersionInfoTransformer>();
            });

        }
        return services;
    }
}