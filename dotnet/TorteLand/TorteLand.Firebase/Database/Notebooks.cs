﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Contracts;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Notebooks.Models;
using TorteLand.Core.Contracts.Storage;
using TorteLand.Firebase.Integration;
using TorteLand.Utils;

namespace TorteLand.Firebase.Database;

internal sealed class Notebooks : INotebooks
{
    private readonly Dictionary<string, IPersistedNotebook> _notebooks = new();

    private readonly IPersistedNotebooksFactory _factory;
    private readonly AsyncLazy<IEntityAcrud> _acrud;

    public Notebooks(IPersistedNotebooksFactory factory, IEntityAcrudFactory acrudFactory)
    {
        _factory = factory;
        _acrud = new AsyncLazy<IEntityAcrud>(acrudFactory.Create);
    }

    public async Task<Page<Unique<Note>>> All(int index, Maybe<Pagination> pagination, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        return await notebook.All(pagination, token);
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, Added added, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        return await notebook.Create(added, token);
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, Guid id, bool isRight, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        return await notebook.Create(id, isRight, token);
    }

    public async Task<Maybe<Note>> Read(int index, int id, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        return await notebook.Read(id, token);
    }

    public async Task Update(int index, int id, string name, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        await notebook.Update(id, name, token);
    }

    public async Task Delete(int index, int id, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        await notebook.Delete(id, token);
    }

    public async Task<Either<byte, int>> Increment(int index, int id, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        return await notebook.Increment(id, token);
    }
    public async Task<Either<byte, int>> Decrement(int index, int id, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        return await notebook.Decrement(id, token);
    }

    private async ValueTask<IPersistedNotebook> GetNotebook(int index, CancellationToken token)
    {
        var acrud = await _acrud;
        var (id, _) = await acrud.Read(index, token);

        if (_notebooks.TryGetValue(id, out var notebook))
            return notebook;

        var created = _factory.Create(id);
        _notebooks.Add(id, created);
        return created;
    }
}