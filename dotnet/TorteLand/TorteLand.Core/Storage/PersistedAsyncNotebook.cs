using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

namespace TorteLand.Core.Storage;

internal sealed class PersistedAsyncNotebook : IAsyncNotebook
{
    private readonly IStorage _storage;

    private Either<IAsyncNotebookFactory, IAsyncNotebook> _origin;

    public PersistedAsyncNotebook(IStorage storage, Either<IAsyncNotebookFactory, IAsyncNotebook> origin)
    {
        _storage = storage;
        _origin = origin;
    }

    public async IAsyncEnumerable<Unique<Note>> All([EnumeratorCancellation] CancellationToken token)
    {
        var origin = await GetOrigin(token);
        await foreach (var note in origin.All(token))
            yield return note;
    }

    public async Task<Either<int, Segment>> Add(
        string value,
        Maybe<ResolvedSegment> segment,
        CancellationToken token)
    {
        var origin = await GetOrigin(token);
        var copy = await origin.Clone(token);
        var added = await copy.Add(value, segment, token);

        await added.MatchAsync(
            _ => SaveChanges(_, copy, token),
            _ => Task.CompletedTask);

        return added;
    }

    public async Task<IAsyncNotebook> Clone(CancellationToken token)
    {
        var origin = await GetOrigin(token);
        var clone = await origin.Clone(token);
        var notebook = new Right<IAsyncNotebookFactory, IAsyncNotebook>(clone);

        return new PersistedAsyncNotebook(_storage, notebook);
    }

    public async Task<Note> ToNote(int key, CancellationToken token)
    {
        var origin = await GetOrigin(token);
        return await origin.ToNote(key, token);
    }

    private async Task SaveChanges(int key, IAsyncNotebook copy, CancellationToken token)
    {
        var transaction = _storage.StartTransaction();
        await WriteChanges(transaction, key, copy, token);
        await transaction.Save(token);

        _origin = new Right<IAsyncNotebookFactory, IAsyncNotebook>(copy);
    }

    private static async Task WriteChanges(
        ITransaction transaction,
        int key,
        IAsyncNotebook copy,
        CancellationToken token)
    {
        var created = await copy.ToNote(key, token);

        transaction.Create(created);

        var changes = copy
                      .All(token)
                      .Where(_ => _.Id.CompareTo(key) > 0)
                      .WithCancellation(token);

        await foreach (var change in changes)
        {
            change.Value
                  ._(transaction.ToEntity)
                  .Update(change.Value.Weight);
        }
    }

    private async ValueTask<IAsyncNotebook> GetOrigin(CancellationToken token)
    {
        var notebook = await  _origin.MatchAsync(
            _ => CreateNotebook(_, token),
            Task.FromResult);

        return notebook;
    }

    private async Task<IAsyncNotebook> CreateNotebook(IAsyncNotebookFactory factory, CancellationToken token)
    {
        var notes = await _storage.StartTransaction().All(token).ToArrayAsync(token);
        return await factory.Create(notes, token);
    }
}