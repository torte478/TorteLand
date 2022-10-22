using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;
using TorteLand.Firebase.Integration;

namespace TorteLand.Firebase.Database;

internal sealed class Storage : IStorage
{
    private readonly string _id;
    private readonly INotebookEntityAcrud _notebookEntityAcrud;

    public Storage(string id, INotebookEntityAcrud notebookEntityAcrud)
    {
        _id = id;
        _notebookEntityAcrud = notebookEntityAcrud;
    }

    public async Task Save(IReadOnlyCollection<Unique<Note>> notes)
    {
        var notebook = await _notebookEntityAcrud.Read(_id);

        await notes
            .OrderBy(_ => _.Value.Weight)
            .Select(_ => _.Value.Text)
            .ToArray()
            ._(_ => notebook with { Notes = _ })
            ._(_ => _notebookEntityAcrud.Update(_id, _));
    }

    public Task DeleteAll()
        => Save(Array.Empty<Unique<Note>>());

    public async Task<IReadOnlyCollection<Note>> All()
    {
        var notebook = await _notebookEntityAcrud.Read(_id);

        return notebook
               .Notes
               .Select((x, i) => new Note(x, i))
               .ToArray();
    }
}