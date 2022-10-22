using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;
using TorteLand.Firebase.Integration;

namespace TorteLand.Firebase.Database;

internal sealed class Notebooks : INotebooks
{
    private readonly Dictionary<string, IQuestionableNotebook> _notebooks = new();

    private readonly INotebookFactory _factory;
    private readonly AsyncLazy<INotebookEntityAcrud> _acrud;

    public Notebooks(INotebookFactory factory, INotebookEntityAcrudFactory acrudFactory)
    {
        _factory = factory;
        _acrud = new AsyncLazy<INotebookEntityAcrud>(acrudFactory.Create);
    }

    public async Task<Page<Unique<Note>>> Read(int index, Maybe<Pagination> pagination, CancellationToken token)
    {
        var notebook = await GetNotebook(index);
        return await notebook.All(pagination, token);
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, IReadOnlyCollection<string> values, CancellationToken token)
    {
        var notebook = await GetNotebook(index);
        return await notebook.Add(values, token);
    }

    public async Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, Guid id, bool isRight, CancellationToken token)
    {
        var notebook = await GetNotebook(index);
        return await notebook.Add(id, isRight, token);
    }

    public async Task Rename(int index, int id, string text, CancellationToken token)
    {
        var notebook = await GetNotebook(index);
        await notebook.Rename(id, text, token);
    }

    public async Task Delete(int index, int key, CancellationToken token)
    {
        var notebook = await GetNotebook(index);
        await notebook.Delete(key, token);
    }

    private async ValueTask<IQuestionableNotebook> GetNotebook(int index)
    {
        var acrud = await _acrud;
        var (id, _) = await acrud.Read(index);

        if (_notebooks.TryGetValue(id, out var notebook))
            return notebook;

        var created = await _factory.Create(id);
        _notebooks.Add(id, created);
        return created;
    }
}