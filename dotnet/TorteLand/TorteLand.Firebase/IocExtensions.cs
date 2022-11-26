using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Firebase.Database;
using TorteLand.Firebase.Integration;

namespace TorteLand.Firebase;

public static class IocExtensions
{
    public static IServiceCollection AddFirebase(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddHttpClient()
                .RemoveAll<IHttpMessageHandlerBuilderFilter>();

        // TODOv2: to extension with validation
        services.Configure<FirebaseSettings>(
            configuration.GetSection(nameof(FirebaseSettings)));
        
        services.AddSingleton<IFirebaseClientFactory, FirebaseClientFactory>();
        services.AddSingleton<IEntityAcrudFactory, EntityAcrudFactory>();
        services.AddSingleton<INotebooks, Notebooks>();
        services.AddSingleton<INotebooksAcrud, NotebooksAcrud>();
        services.AddSingleton<IPersistedNotebooksFactory, PersistedNotebooksFactory>();

        return services;
    }
}