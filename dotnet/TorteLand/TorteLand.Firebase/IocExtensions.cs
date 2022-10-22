using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Firebase.Database;
using TorteLand.Firebase.Integration;

namespace TorteLand.Firebase;

public static class IocExtensions
{
    public static IServiceCollection AddFirebase(this IServiceCollection services, ConfigurationManager configuration)
    {
        var config = configuration.GetSection("Firebase");

        services.AddSingleton<IFirebaseClientFactory>(
            _ => new FirebaseClientFactory(
                new Credentials(
                    Url: config["Url"],
                    Email: config["Email"],
                    Password: config["Password"],
                    ApiKey: config["ApiKey"])));

        services.AddSingleton<INotebookEntityAcrudFactory>(
            _ => new NotebookEntityAcrudFactory(
                root: config["Root"],
                factory: _.GetRequiredService<IFirebaseClientFactory>()));

        services.AddSingleton<INotebooks, Notebooks>();
        services.AddSingleton<INotebooksAcrud, NotebooksAcrud>();
        services.AddSingleton<INotebookFactory, NotebookFactory>();

        services.AddHostedService(
            _ => new FileStorageToFirebaseMigration(
                path: configuration.GetSection("FileStorage")["Path"],
                factory: _.GetRequiredService<INotebookEntityAcrudFactory>()));

        return services;
    }
}