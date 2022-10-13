using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;
using TorteLand.Core.Storage;

namespace TorteLand.Core.Notebooks;

internal sealed class Factory : IFactory
{
    private readonly IStorage _storage;
    private readonly INotebookFactory _factory;

    public Factory(IStorage storage, INotebookFactory factory)
    {
        _storage = storage;
        _factory = factory;
    }

    public ITransactionNotebook Create()
    {
        var persisted = new PersistedAsyncNotebook(_storage, new Left<INotebookFactory, INotebook>(_factory));
        return new TransactionNotebook(persisted);
    }
}