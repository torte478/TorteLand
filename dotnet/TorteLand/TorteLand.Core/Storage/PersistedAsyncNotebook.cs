using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Notebooks;

namespace TorteLand.Core.Storage;

internal sealed class PersistedAsyncNotebook : IAsyncNotebook
{
    private readonly IStorage _storage;

    private IAsyncNotebook _origin;

    public PersistedAsyncNotebook(IAsyncNotebook origin, IStorage storage)
    {
        _origin = origin;
        _storage = storage;
    }

    public IAsyncEnumerator<Unique<Note>> GetAsyncEnumerator(CancellationToken token)
        => _origin.GetAsyncEnumerator(token);

    public async Task<Either<int, Segment>> Add(
        string value,
        Maybe<ResolvedSegment> segment,
        CancellationToken token)
    {
        var copy = _origin.Clone();
        var added = await copy.Add(value, segment, token);

        await added.MatchAsync(
            _ => SaveChanges(_, copy, token),
            _ => Task.CompletedTask);

        return added;
    }

    public IAsyncNotebook Clone()
        => new PersistedAsyncNotebook(
            _origin.Clone(),
            _storage);

    public Task<Note> ToNote(int key)
        => _origin.ToNote(key);

    private async Task SaveChanges(int key, IAsyncNotebook copy, CancellationToken token)
    {
        var transaction = _storage.StartTransaction();
        await WriteChanges(transaction, key, copy, token);
        await transaction.Save(token);

        _origin = copy;
    }

    private static async Task WriteChanges(
        ITransaction transaction,
        int key,
        IAsyncNotebook copy,
        CancellationToken token)
    {
        var created = await copy.ToNote(key);

        transaction.Create(created);

        var changes = copy
                      .Where(_ => _.Id.CompareTo(key) > 0)
                      .WithCancellation(token);

        await foreach (var change in changes)
        {
            change.Value
                  ._(transaction.ToEntity)
                  .Update(change.Value.Weight);
        }
    }
}