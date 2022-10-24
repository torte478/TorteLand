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

        // TODO : to IOptions
        services.AddSingleton<IFirebaseClientFactory>(
            _ => new FirebaseClientFactory(
                new Credentials(
                    Url: config["Url"],
                    Email: config["Email"],
                    Password: config["Password"],
                    ApiKey: config["ApiKey"])));

        services.AddSingleton<IEntityAcrudFactory>(
            _ => new EntityAcrudFactory(
                root: config["Root"],
                factory: _.GetRequiredService<IFirebaseClientFactory>()));

        services.AddSingleton<INotebooks, Notebooks>();
        services.AddSingleton<INotebooksAcrud, NotebooksAcrud>();
        services.AddSingleton<INotebookFactory, NotebookFactory>();

        return services;
    }
}