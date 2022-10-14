using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Contracts.Storage;

public interface ITransaction
{
    void Create(Note note);
    IEntity ToEntity(Note note);
    void Update(Note note);
    Task Save(CancellationToken token);
    IAsyncEnumerable<Note> All(CancellationToken token);
    void Delete(Note note);
    Task DeleteAll(CancellationToken token);
}