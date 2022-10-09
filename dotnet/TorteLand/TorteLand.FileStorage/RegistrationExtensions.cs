using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TorteLand.Core;

namespace TorteLand.FileStorage;

public static class RegistrationExtensions
{
    public static IServiceCollection AddStorage(this IServiceCollection services, ConfigurationManager configuration)
        => services.AddSingleton<IAcrud<int, string>>(
            new FileStorage(
                root: configuration["FileStorageDirectory"]));
}