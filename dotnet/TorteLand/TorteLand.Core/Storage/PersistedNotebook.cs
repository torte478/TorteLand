using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public async IAsyncEnumerable<Unique<Note>> All([EnumeratorCancellation] CancellationToken token)
    {
        var origin = await GetOrigin(token);
        foreach (var note in origin)
            yield return note;
    }

    public async Task<Either<int, Segment>> Add(
        string value,
        Maybe<ResolvedSegment> segment,
        CancellationToken token)
    {
        var origin = await GetOrigin(token);
        var copy = origin.Clone();
        var added = copy.Add(value, segment);

        await added.MatchAsync(
            _ => SaveChanges(_, copy, token),
            _ => Task.CompletedTask);

        return added;
    }

    public async Task<IAsyncNotebook> Clone(CancellationToken token)
    {
        var origin = await GetOrigin(token);
        var clone = origin.Clone();
        var notebook = new Right<INotebookFactory, INotebook>(clone);

        return new PersistedNotebook(_storage, notebook);
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
        await transaction.Save(token);

        _origin = new Right<INotebookFactory, INotebook>(copy);
        return deleted;
    }

    public Task DeleteAll(CancellationToken token)
        => _storage.StartTransaction().DeleteAll(token);

    public async Task<Note> ToNote(int key, CancellationToken token)
    {
        var origin = await GetOrigin(token);
        return origin.ToNote(key);
    }

    private async Task SaveChanges(int key, INotebook copy, CancellationToken token)
    {
        var transaction = _storage.StartTransaction();
        WriteChanges(transaction, key, copy);
        await transaction.Save(token);

        _origin = new Right<INotebookFactory, INotebook>(copy);
    }

    private static void WriteChanges(
        ITransaction transaction,
        int key,
        INotebook copy)
    {
        var created = copy.ToNote(key);

        transaction.Create(created);

        var changes = copy.Where(_ => _.Id.CompareTo(key) > 0);

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