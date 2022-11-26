using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;
using TorteLand.Firebase.Integration;

namespace TorteLand.Firebase.Database;

internal sealed class Storage : IStorage
{
    private readonly string _id;
    private readonly IEntityAcrud _entityAcrud;

    public Storage(string id, IEntityAcrud entityAcrud)
    {
        _id = id;
        _entityAcrud = entityAcrud;
    }

    public async Task Save(IReadOnlyCollection<Unique<Note>> notes, CancellationToken token)
    {
        var notebook = await _entityAcrud.Read(_id, token);

        await notes
            .OrderBy(_ => _.Value.Weight)
            .Select(_ => _.Value.Text)
            .ToArray()
            ._(_ => notebook with { Notes = _ })
            ._(_ => _entityAcrud.Update(_id, _, token));
    }

    public async Task<IReadOnlyCollection<Note>> All(CancellationToken token)
    {
        var notebook = await _entityAcrud.Read(_id, token);

        return notebook
               .Notes
               .Select((x, i) => new Note(x, i))
               .ToArray();
    }
}