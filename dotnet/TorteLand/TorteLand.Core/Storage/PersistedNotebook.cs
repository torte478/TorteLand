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
using TorteLand.Core.Contracts.Notebooks.Models;
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
                var notes = await _storage.All(default); // TODO: cancellation
                return factory.Create(notes);
            });
    }

    public async Task<Page<Unique<Note>>> All(Maybe<Pagination> pagination, CancellationToken token)
    {
        var origin = await _origin;
        return origin.All(pagination);
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Create(
        Added added, 
        CancellationToken token)
    {
        // TODO: chore
        var origin = await _origin;
        
        var iteration = origin.Create(added);
        await iteration.Result.MatchAsync(
            _ => SaveChanges(iteration.Notebook, token),
            _ => Task.CompletedTask);
        
        SetOrigin(iteration.Notebook);
        return iteration.Result;
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Create(Guid id, bool isRight, CancellationToken token)
    {
        var origin = await _origin;

        var iteration = origin.Create(id, isRight);

        await iteration.Result.MatchAsync(
            _ => SaveChanges(iteration.Notebook, token),
            _ => Task.CompletedTask);
        
        SetOrigin(iteration.Notebook);

        return iteration.Result;
    }

    public async Task Update(int key, string name, CancellationToken token)
    {
        var origin = await _origin;
        var updated = origin.Update(key, name);

        await SaveChanges(updated, token);
        SetOrigin(updated);
    }

    public async Task Delete(int key, CancellationToken token)
    {
        var origin = await _origin;
        var updated = origin.Delete(key);

        await SaveChanges(updated, token);
        SetOrigin(updated);
    }

    public async Task<Either<byte, int>> Increment(int key, CancellationToken token)
    {
        var origin = await _origin;
        var (updated, result) = origin.Increment(key);

        await SaveChanges(updated, token);
        SetOrigin(updated);
        return result;
    }
    
    public async Task<Either<byte, int>> Decrement(int key, CancellationToken token)
    {
        var origin = await _origin;
        var (updated, result) = origin.Decrement(key);

        await SaveChanges(updated, token);
        SetOrigin(updated);
        return result;
    }

    public async Task<Maybe<Note>> Read(int key, CancellationToken token)
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