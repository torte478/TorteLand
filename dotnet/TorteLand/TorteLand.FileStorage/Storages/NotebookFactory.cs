using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.FileStorage.Storages;

internal sealed class NotebookFactory : INotebookFactory
{
    private readonly Core.Contracts.Factories.INotebookFactory _notebookFactory;
    private readonly IPersistedNotebookFactory _persistedNotebookFactory;
    private readonly IQuestionableNotebookFactory _questionableNotebookFactory;
    private readonly IStorageFactory _storageFactory;

    public NotebookFactory(
        Core.Contracts.Factories.INotebookFactory notebookFactory,
        IPersistedNotebookFactory persistedNotebookFactory,
        IQuestionableNotebookFactory questionableNotebookFactory,
        IStorageFactory storageFactory)
    {
        _notebookFactory = notebookFactory;
        _persistedNotebookFactory = persistedNotebookFactory;
        _questionableNotebookFactory = questionableNotebookFactory;
        _storageFactory = storageFactory;
    }

    public IQuestionableNotebook Create(string path)
    {
        var storage = _storageFactory.Create(path);
        var persisted = _persistedNotebookFactory.Create(
            storage,
            new Left<Core.Contracts.Factories.INotebookFactory, INotebook>(_notebookFactory));
        return _questionableNotebookFactory.Create(persisted);
    }
}