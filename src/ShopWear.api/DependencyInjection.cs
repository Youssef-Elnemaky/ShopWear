using ShopWear.Application;

namespace ShopWear.api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        // API level


        // register application services (it internally registers data access services as well)
        services.AddApplication(configuration);

        return services;
    }
}