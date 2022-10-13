using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

namespace TorteLand.Core.Notebooks;

// TODO
internal sealed class Notebooks : INotebooks
{
    private readonly ITransactionNotebook _notebook;

    public Notebooks(IFactory factory)
    {
        _notebook = factory.Create();
    }

    public IAsyncEnumerable<Unique<Note>> All(int index, CancellationToken token)
        => _notebook.All(token);

    public int Create() => 0;

    public Task<Either<int, Transaction>> Add(int index, string value, CancellationToken token)
        => _notebook.Add(value, token);

    public Task<Either<int, Transaction>> Add(int index, Guid id, bool isRight, CancellationToken token)
        => _notebook.Add(id, isRight, token);

    public Task Delete(int index, int key, CancellationToken token)
        => _notebook.Delete(key, token);
}