using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Firebase.Integration;

namespace TorteLand.Firebase.Database;

internal sealed class NotebookFactory : INotebookFactory
{
    private readonly IEntityAcrudFactory _entityAcrudFactory;
    private readonly IPersistedNotebookFactory _persistedNotebookFactory;
    private readonly IQuestionableNotebookFactory _questionableNotebookFactory;

    public NotebookFactory(
        IPersistedNotebookFactory persistedNotebookFactory,
        IQuestionableNotebookFactory questionableNotebookFactory,
        IEntityAcrudFactory entityAcrudFactory)
    {
        _questionableNotebookFactory = questionableNotebookFactory;
        _entityAcrudFactory = entityAcrudFactory;
        _persistedNotebookFactory = persistedNotebookFactory;
        _entityAcrudFactory = entityAcrudFactory;
    }

    public async Task<IPersistedNotebook> Create(string id)
    {
        var acrud = await _entityAcrudFactory.Create();
        var storage = new Storage(id, acrud);

        // TODO: can be replace to TorteLand.Core
        var factory = new Left<IQuestionableNotebookFactory, IQuestionableNotebook>(_questionableNotebookFactory);
        return _persistedNotebookFactory.Create(storage, factory);
    }
}