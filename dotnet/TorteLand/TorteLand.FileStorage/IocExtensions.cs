using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TorteLand.Core.Contracts;
using TorteLand.FileStorage.Storages;

namespace TorteLand.FileStorage;

public static class IocExtensions
{
    public static IServiceCollection AddFileStorage(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton<ITransactionFactory, TransactionFactory>();
        services.AddSingleton<IStorage>(
            provider => new Storage(
                file: configuration.GetSection("FileStorage")["Path"],
                factory: provider.GetRequiredService<ITransactionFactory>()));

        return services;
    }
}