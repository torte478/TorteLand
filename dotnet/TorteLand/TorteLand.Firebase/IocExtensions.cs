using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Extensions;
using TorteLand.Firebase.Database;
using TorteLand.Firebase.Integration;
using TorteLand.Firebase.Integration.Tokens;
using TorteLand.Firebase.Migrations;

namespace TorteLand.Firebase;

public static class IocExtensions
{
    public static IServiceCollection AddFirebase(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddHttpClient()
                .RemoveAll<IHttpMessageHandlerBuilderFilter>();

        services.AddSettings<FirebaseSettings>(configuration);

        services.AddSingleton<IExpiredToken, RemoteExpiredToken>();
        services.AddSingleton<IToken, Token>();
        services.AddSingleton<IFirebaseClientFactory, FirebaseClientFactory>();
        services.AddSingleton<IEntityAcrudFactory, EntityAcrudFactory>();
        services.AddSingleton<INotebooks, Notebooks>();
        services.AddSingleton<INotebooksAcrud, NotebooksAcrud>();
        services.AddSingleton<IPersistedNotebooksFactory, PersistedNotebooksFactory>();

        services.AddTransient<IInitialization, MigrationV0V1>();

        return services;
    }
}