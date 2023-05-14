using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks.Models;
using TorteLand.Core.Contracts.Storage;
using TorteLand.Extensions;
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
            .Select(ToNoteEntity)
            .ToArray()
            ._(_ => notebook with { Notes = _ })
            ._(_ => _entityAcrud.Update(_id, _, token));
    }

    public async Task<IReadOnlyCollection<Note>> All(CancellationToken token)
    {
        var notebook = await _entityAcrud.Read(_id, token);

        return notebook
               .Notes
               .Select(ToNote)
               .ToArray();
    }

    private static NoteEntity ToNoteEntity(Unique<Note> note)
        => new(
            Text: note.Value.Text,
            Pluses: note.Value.Pluses.Match(
                _ => _,
                () => 0));

    private static Note ToNote(NoteEntity entity, int index)
        => new(
            Text: entity.Text,
            Weight: index,
            Pluses: entity.Pluses > 0
                        ? Maybe.Some((byte)entity.Pluses)
                        : Maybe.None<byte>());
}