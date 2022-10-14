using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Contracts.Storage;

public interface ITransaction
{
    IEntity Create(Note note);
    IEntity ToEntity(Note note);
    void Update(IEntity entity);
    Task SaveChanges(CancellationToken token);
    IAsyncEnumerable<Note> All(CancellationToken token);
    void Delete(IEntity entity);
    Task DeleteAll(CancellationToken token);
}