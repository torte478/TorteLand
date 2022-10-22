using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Storage;

internal sealed class PersistedNotebook : IAsyncNotebook
{
    private readonly IStorage _storage;

    private Either<INotebookFactory, INotebook> _origin;

    public PersistedNotebook(IStorage storage, Either<INotebookFactory, INotebook> origin)
    {
        _storage = storage;
        _origin = origin;
    }

    public async Task<Page<Unique<Note>>> All(Maybe<Pagination> pagination, CancellationToken token)
    {
        var origin = await GetOrigin(token);
        return origin.All(pagination);
    }

    public async Task<Either<IReadOnlyCollection<int>, Segment>> Add(
        IReadOnlyCollection<string> values,
        Maybe<ResolvedSegment> segment,
        CancellationToken token)
    {
        var origin = await GetOrigin(token);
        var copy = origin.Clone();
        var added = copy.Add(values, segment);

        await added.MatchAsync(
            _ => SaveChanges(_, copy, token),
            _ => Task.CompletedTask);

        return added;
    }

    public async Task Rename(int key, string text, CancellationToken token)
    {
        var origin = await GetOrigin(token);
        var copy = origin.Clone();
        var updated = copy.ToNote(key);
        copy.Rename(key, text);

        var transaction = _storage.StartTransaction();
        var entity = transaction.ToEntity(updated);
        entity.Update(text);
        await transaction.SaveChanges(token);

        _origin = new Right<INotebookFactory, INotebook>(copy);
    }

    public async Task<Note> Delete(int key, CancellationToken token)
    {
        var origin = await GetOrigin(token);
        var copy = origin.Clone();
        var deleted =  copy.Delete(key);

        var transaction = _storage.StartTransaction();
        var entity = transaction.ToEntity(deleted);
        entity.Delete();

        foreach (var note in copy)
        {
            var updated = transaction.ToEntity(note.Value);
            updated.Update(note.Value.Weight);
        }
        await transaction.SaveChanges(token);

        _origin = new Right<INotebookFactory, INotebook>(copy);
        return deleted;
    }

    public Task DeleteAll(CancellationToken token)
    {
        _storage.StartTransaction().DeleteAll(token);
        return Task.CompletedTask;
    }

    public async Task<Note> ToNote(int key, CancellationToken token)
    {
        var origin = await GetOrigin(token);
        return origin.ToNote(key);
    }

    private async Task SaveChanges(IReadOnlyCollection<int> keys, INotebook copy, CancellationToken token)
    {
        var transaction = _storage.StartTransaction();
        WriteChanges(transaction, keys, copy);
        await transaction.SaveChanges(token);

        _origin = new Right<INotebookFactory, INotebook>(copy);
    }

    private static void WriteChanges(
        ITransaction transaction,
        IReadOnlyCollection<int> keys,
        INotebook copy)
    {
        foreach (var key in keys)
            key
                ._(copy.ToNote)
                ._(transaction.Create);

        var max = keys.Max();

        var changes = copy.Where(_ => _.Id > max);

        foreach (var change in changes)
        {
            change.Value
                  ._(transaction.ToEntity)
                  .Update(change.Value.Weight);
        }
    }

    private async ValueTask<INotebook> GetOrigin(CancellationToken token)
    {
        var notebook = await  _origin.MatchAsync(
            _ => CreateNotebook(_, token),
            Task.FromResult);

        return notebook;
    }

    private async Task<INotebook> CreateNotebook(INotebookFactory factory, CancellationToken token)
    {
        var notes = await _storage.StartTransaction().All(token).ToArrayAsync(token);
        return factory.Create(notes);
    }
}