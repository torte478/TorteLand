using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Storage;

namespace TorteLand.Core.Notebooks;

internal sealed class Factory : IFactory
{
    private readonly IStorage _storage;
    private readonly IAsyncNotebookFactory _factory;

    public Factory(IStorage storage, IAsyncNotebookFactory factory)
    {
        _storage = storage;
        _factory = factory;
    }

    public ITransactionNotebook Create()
    {
        var persisted = new PersistedAsyncNotebook(_storage, new Left<IAsyncNotebookFactory, IAsyncNotebook>(_factory));
        return new TransactionNotebook(persisted);
    }
}