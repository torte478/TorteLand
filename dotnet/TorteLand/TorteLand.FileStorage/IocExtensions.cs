using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.FileStorage.Storages;

namespace TorteLand.FileStorage;

public static class IocExtensions
{
    public static IServiceCollection AddFileStorage(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton<ITransactionFactory, TransactionFactory>();
        services.AddSingleton<IStorageFactory, StorageFactory>();
        services.AddSingleton<INotebookFactory, NotebookFactory>();

        services.AddSingleton<INotebooks>(
            provider => new FileNotebooks(
                path: configuration.GetSection("FileStorage")["data"],
                factory: provider.GetRequiredService<INotebookFactory>()));

        return services;
    }
}