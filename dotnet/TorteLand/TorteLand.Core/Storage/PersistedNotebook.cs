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

namespace TorteLand.Core.Storage;

internal sealed class PersistedNotebook : IPersistedNotebook
{
    private readonly IStorage _storage;

    private Either<IQuestionableNotebookFactory, IQuestionableNotebook> _origin; // TODO : toAsyncLazy 

    public PersistedNotebook(
        IStorage storage, 
        Either<IQuestionableNotebookFactory, IQuestionableNotebook> origin)
    {
        _storage = storage;
        _origin = origin;
    }

    public async Task<Page<Unique<Note>>> All(Maybe<Pagination> pagination, CancellationToken token)
    {
        var origin = await GetOrigin(token);
        return origin.All(pagination);
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Create(
        IReadOnlyCollection<string> values, 
        CancellationToken token)
    {
        var origin = await GetOrigin(token);
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
        var origin = await GetOrigin(token);
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
        var origin = await GetOrigin(token);
        var copy = origin.Clone();
        copy.Update(key, name);

        await SaveChanges(copy, token);
        SetOrigin(copy);
    }

    public async Task Delete(int key, CancellationToken token)
    {
        var origin = await GetOrigin(token);
        var copy = origin.Clone();
        copy.Delete(key);

        await SaveChanges(copy, token);
        SetOrigin(copy);
    }

    public async Task<Maybe<string>> Read(int key, CancellationToken token)
    {
        var origin = await GetOrigin(token);
        return origin.Read(key);
    }

    private Task SaveChanges(IQuestionableNotebook copy, CancellationToken token)
        => _storage.Save(copy.ToArray(), token);

    private void SetOrigin(IQuestionableNotebook next)
    {
        _origin = new Right<IQuestionableNotebookFactory, IQuestionableNotebook>(next);
    }

    private async ValueTask<IQuestionableNotebook> GetOrigin(CancellationToken token)
    {
        var notebook = await _origin.MatchAsync(
            _ => CreateNotebook(_, token),
            Task.FromResult);

        return notebook;
    }

    private async Task<IQuestionableNotebook> CreateNotebook(IQuestionableNotebookFactory factory, CancellationToken token)
    {
        var notes = await _storage.All(token);
        var notebook = factory.Create(notes);
        _origin = new Right<IQuestionableNotebookFactory, IQuestionableNotebook>(notebook);
        return notebook;
    }
}