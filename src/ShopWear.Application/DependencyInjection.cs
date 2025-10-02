using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopWear.Application.Services.Category;
using ShopWear.Application.Services.Products;
using ShopWear.DataAccess;

namespace ShopWear.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // application services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        // data access services
        services.AddDataAccess(configuration);

        return services;
    }
}