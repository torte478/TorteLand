using Microsoft.Extensions.DependencyInjection;
using TorteLand.Core.Contracts;
using TorteLand.Core.Notebooks;
using TorteLand.Core.Storage;

namespace TorteLand.Core;

public static class IocExtensions
{
    public static IServiceCollection AddCoreLogic(this IServiceCollection services)
    {
        services.AddSingleton<IEntityFactory, EntityFactory>();
        services.AddSingleton<IFactory, Factory>();
        services.AddSingleton<INotebooks, Notebooks.Notebooks>();

        return services;
    }
}