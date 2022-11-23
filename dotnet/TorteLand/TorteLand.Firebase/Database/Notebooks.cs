using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;
using TorteLand.Firebase.Integration;

namespace TorteLand.Firebase.Database;

internal sealed class Notebooks : INotebooks
{
    private readonly Dictionary<string, IPersistedNotebook> _notebooks = new();

    private readonly INotebookFactory _factory;
    private readonly AsyncLazy<IEntityAcrud> _acrud;

    public Notebooks(INotebookFactory factory, IEntityAcrudFactory acrudFactory)
    {
        _factory = factory;
        _acrud = new AsyncLazy<IEntityAcrud>(acrudFactory.Create);
    }

    public async Task<Page<Unique<Note>>> All(int index, Maybe<Pagination> pagination, CancellationToken token)
    {
        var notebook = await GetNotebook(index);
        return await notebook.All(pagination, token);
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, IReadOnlyCollection<string> values, CancellationToken token)
    {
        var notebook = await GetNotebook(index);
        return await notebook.Create(values, token);
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, Guid id, bool isRight, CancellationToken token)
    {
        var notebook = await GetNotebook(index);
        return await notebook.Create(id, isRight, token);
    }

    public async Task<Maybe<string>> Read(int index, int key, CancellationToken token)
    {
        var notebook = await GetNotebook(index);
        return await notebook.Read(key, token);
    }

    public async Task Update(int index, int id, string name, CancellationToken token)
    {
        var notebook = await GetNotebook(index);
        await notebook.Update(id, name, token);
    }

    public async Task Delete(int index, int key, CancellationToken token)
    {
        var notebook = await GetNotebook(index);
        await notebook.Delete(key, token);
    }

    private async ValueTask<IPersistedNotebook> GetNotebook(int index)
    {
        var acrud = await _acrud;
        var (id, _) = await acrud.Read(index);

        if (_notebooks.TryGetValue(id, out var notebook))
            return notebook;

        var created = await _factory.Create(id);
        _notebooks.Add(id, created);
        return created;
    }
}