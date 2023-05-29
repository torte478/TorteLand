using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Firebase.Integration;

namespace TorteLand.Firebase.Database;

internal sealed class PersistedNotebooksFactory : IPersistedNotebooksFactory
{
    private readonly IEntityAcrudFactory _entityAcrudFactory;
    private readonly IPersistedNotebookFactory _persistedNotebookFactory;
    private readonly IQuestionableNotebookFactory _questionableNotebookFactory;

    public PersistedNotebooksFactory(
        IPersistedNotebookFactory persistedNotebookFactory,
        IQuestionableNotebookFactory questionableNotebookFactory,
        IEntityAcrudFactory entityAcrudFactory)
    {
        _questionableNotebookFactory = questionableNotebookFactory;
        _entityAcrudFactory = entityAcrudFactory;
        _persistedNotebookFactory = persistedNotebookFactory;
        _entityAcrudFactory = entityAcrudFactory;
    }

    public IPersistedNotebook Create(string id)
    {
        var acrud = _entityAcrudFactory.Create();
        var storage = new Storage(id, acrud);
        return _persistedNotebookFactory.Create(storage, _questionableNotebookFactory);
    }
}