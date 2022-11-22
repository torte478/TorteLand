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
        var origin = await GetOrigin();
        return origin.All(pagination);
    }

    public async Task<Either<IReadOnlyCollection<int>, Segment>> Add(
        IReadOnlyCollection<string> values,
        Maybe<ResolvedSegment> segment,
        CancellationToken token)
    {
        var origin = await GetOrigin();
        var copy = origin.Clone();
        var added = copy.Add(values, segment);

        await added.MatchAsync(
            _ => SaveChanges(copy),
            _ => Task.CompletedTask);

        return added;
    }

    public async Task Update(int key, string name, CancellationToken token)
    {
        var origin = await GetOrigin();
        var copy = origin.Clone();
        copy.Update(key, name);

        await SaveChanges(copy);
    }

    // TODO : add token using
    public async Task<Note> Delete(int key, CancellationToken token)
    {
        var origin = await GetOrigin();
        var copy = origin.Clone();
        var deleted = copy.Delete(key);

        await SaveChanges(copy);
        return deleted;
    }

    public Task DeleteAll(CancellationToken token)
        => _storage.DeleteAll();

    public async Task<Maybe<string>> Read(int key, CancellationToken token)
    {
        var origin = await GetOrigin();
        return origin.Read(key);
    }

    public async Task<Note> ToNote(int key, CancellationToken token)
    {
        var origin = await GetOrigin();
        return origin.ToNote(key);
    }

    private async Task SaveChanges(INotebook copy)
    {
        await _storage.Save(copy.ToArray());
        _origin = new Right<INotebookFactory, INotebook>(copy);
    }

    private async ValueTask<INotebook> GetOrigin()
    {
        var notebook = await _origin.MatchAsync(
            CreateNotebook,
            Task.FromResult);

        return notebook;
    }

    private async Task<INotebook> CreateNotebook(INotebookFactory factory)
    {
        var notes = await _storage.All();
        return factory.Create(notes);
    }
}