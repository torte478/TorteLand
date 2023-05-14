using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks.Models;
using TorteLand.Core.Notebooks;
using TorteLand.Core.Storage;
using TorteLand.Extensions;

namespace TorteLand.Core;

public static class IocExtensions
{
    public static IServiceCollection AddCoreLogic(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSettings<NotebookSettings>(configuration);

        services.AddSingleton<INotebookFactory, NotebookFactory>();
        services.AddSingleton<IPersistedNotebookFactory, PersistedNotebookFactory>();
        services.AddSingleton<IQuestionableNotebookFactory, QuestionableNotebookFactory>();

        return services;
    }
}