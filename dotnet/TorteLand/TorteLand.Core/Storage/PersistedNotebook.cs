using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Contracts;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;
using TorteLand.Utils;

namespace TorteLand.Core.Storage;

internal sealed class PersistedNotebook : IPersistedNotebook
{
    private readonly IStorage _storage;

    private AsyncLazy<IQuestionableNotebook> _origin;

    public PersistedNotebook(
        IStorage storage, 
        IQuestionableNotebookFactory factory)
    {
        _storage = storage;
        _origin = new AsyncLazy<IQuestionableNotebook>(
            async () =>
            {
                var notes = await _storage.All(default); // TODOv2: cancellation
                return factory.Create(notes);
            });
    }

    public async Task<Page<Unique<Note>>> All(Maybe<Pagination> pagination, CancellationToken token)
    {
        var origin = await _origin;
        return origin.All(pagination);
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Create(
        IReadOnlyCollection<string> values, 
        CancellationToken token)
    {
        var origin = await _origin;
        var copy = origin.Clone();
        var added = copy.Create(values);

        await added.MatchAsync(
            _ => SaveChanges(copy, token),
            _ => Task.CompletedTask);
        
        SetOrigin(copy);

        return added;
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Create(Guid id, bool isRight, CancellationToken token)
    {
        var origin = await _origin;
        var copy = origin.Clone();
        var added = copy.Create(id, isRight);

        await added.MatchAsync(
            _ => SaveChanges(copy, token),
            _ => Task.CompletedTask);
        
        SetOrigin(copy);

        return added;
    }

    public async Task Update(int key, string name, CancellationToken token)
    {
        var origin = await _origin;
        var copy = origin.Clone();
        copy.Update(key, name);

        await SaveChanges(copy, token);
        SetOrigin(copy);
    }

    public async Task Delete(int key, CancellationToken token)
    {
        var origin = await _origin;
        var copy = origin.Clone();
        copy.Delete(key);

        await SaveChanges(copy, token);
        SetOrigin(copy);
    }

    public async Task<Maybe<string>> Read(int key, CancellationToken token)
    {
        var origin = await _origin;
        return origin.Read(key);
    }

    private Task SaveChanges(IQuestionableNotebook copy, CancellationToken token)
        => _storage.Save(copy.ToArray(), token);

    private void SetOrigin(IQuestionableNotebook next)
    {
        _origin = new AsyncLazy<IQuestionableNotebook>(next);
    }
}