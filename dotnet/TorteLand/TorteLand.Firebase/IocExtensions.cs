using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Firebase.Database;

namespace TorteLand.Firebase;

public static class IocExtensions
{
    public static IServiceCollection AddFirebase(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton<IFirebaseClientFactory>(
            _ =>
            {
                var config = configuration.GetSection("Firebase");

                return new FirebaseClientFactory(
                    new Credentials(
                        Url: config["Url"],
                        Email: config["Email"],
                        Password: config["Password"],
                        ApiKey: config["ApiKey"]));
            });

        services.AddSingleton<INotebooks, Notebooks>();
        services.AddSingleton<INotebooksAcrud, NotebooksAcrud>();

        services.AddSingleton(
            _ => new FileStorageToFirebaseMigration(
                path: configuration.GetSection("FileStorage")["Path"]));

        return services;
    }
}