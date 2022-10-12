using Microsoft.Extensions.DependencyInjection;
using TorteLand.Core.Contracts;
using TorteLand.Core.Notebooks;

namespace TorteLand.Core;

public static class RegisterExtensions
{
    public static IServiceCollection AddCoreLogic(this IServiceCollection services)
    {
        services.AddTransient<IKeyGenerator<int>, IntKeyGenerator>();

        services.AddSingleton<IAsyncNotebookFactory<int, string>, AsyncNotebookFactory<string>>();
        services.AddSingleton<INotebooks<int, int, string>, Notebooks<int, int, string>>();

        return services;
    }
}