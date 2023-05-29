using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TorteLand.Extensions;

public static class ConfigurationExceptions
{
    public static IServiceCollection AddSettings<T>(
        this IServiceCollection services, 
        ConfigurationManager configuration) 
        where T : class
    {
        services.Configure<T>(
            configuration.GetRequiredSection(typeof(T).Name));
        
        return services;
    }
}