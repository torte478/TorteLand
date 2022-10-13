using System.Collections.Generic;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Storage;

namespace TorteLand.Core.Notebooks;

internal sealed class Factory : IFactory
{
    private readonly IStorage _storage;

    public Factory(IStorage storage)
    {
        _storage = storage;
    }

    public IAsyncNotebook Create()
    {
        var notebook = new Notebook(Maybe.None<List<string>>());
        var asyncNotebook = new AsyncNotebook(notebook);
        var persisted = new PersistedAsyncNotebook(asyncNotebook, _storage);
        return persisted;
    }
}