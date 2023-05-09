using Microsoft.Extensions.DependencyInjection;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Notebooks;
using TorteLand.Core.Storage;

namespace TorteLand.Core;

public static class IocExtensions
{
    public static IServiceCollection AddCoreLogic(this IServiceCollection services)
    {
        services.AddSingleton<IClock, Clock>();

        services.AddSingleton<INotebookFactory, NotebookFactory>();
        services.AddSingleton<IPersistedNotebookFactory, PersistedNotebookFactory>();
        services.AddSingleton<IQuestionableNotebookFactory, QuestionableNotebookFactory>();

        return services;
    }
}