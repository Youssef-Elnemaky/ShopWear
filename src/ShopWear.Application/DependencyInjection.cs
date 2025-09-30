using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopWear.Application.Services.Category;
using ShopWear.DataAccess;

namespace ShopWear.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // application services
        services.AddScoped<ICategoryService, CategoryService>();

        // data access services
        services.AddDataAccess(configuration);

        return services;
    }
}