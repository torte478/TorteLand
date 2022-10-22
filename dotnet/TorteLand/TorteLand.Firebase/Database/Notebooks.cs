using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Firebase.Database;

internal sealed class Notebooks : INotebooks
{
    private readonly Dictionary<string, IQuestionableNotebook> _notebooks = new();

    private readonly INotebookFactory _factory;
    private readonly INotebooksAcrud _acrud;

    public Notebooks(INotebookFactory factory, INotebooksAcrud acrud)
    {
        _factory = factory;
        _acrud = acrud;
    }

    public async Task<Page<Unique<Note>>> Read(int index, Maybe<Pagination> pagination, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        return await notebook.All(pagination, token);
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, IReadOnlyCollection<string> values, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        return await notebook.Add(values, token);
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, Guid id, bool isRight, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        return await notebook.Add(id, isRight, token);
    }

    public async Task Rename(int index, int id, string text, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        await notebook.Rename(id, text, token);
    }

    public async Task Delete(int index, int key, CancellationToken token)
    {
        var notebook = await GetNotebook(index, token);
        await notebook.Delete(key, token);
    }

    private async ValueTask<IQuestionableNotebook> GetNotebook(int index, CancellationToken token)
    {
        var key = await _acrud.Read(index, token);

        if (_notebooks.TryGetValue(key, out var notebook))
            return notebook;

        var created = await _factory.Create(key);
        _notebooks.Add(key, created);
        return created;
    }
}