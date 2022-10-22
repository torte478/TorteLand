using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Contracts.Storage;

public interface ITransaction
{
    IEntity ToEntity(Note note);
    Task SaveChanges(CancellationToken token);

    IAsyncEnumerable<Note> All(CancellationToken token);
    IEntity Create(Note note);
    void Update(IEntity entity);
    void Delete(IEntity entity);
    void DeleteAll(CancellationToken token);
}