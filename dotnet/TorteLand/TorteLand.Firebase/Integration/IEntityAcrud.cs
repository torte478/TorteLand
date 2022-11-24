using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TorteLand.Firebase.Integration;

internal interface IEntityAcrud
{
    Task<IReadOnlyCollection<(string Id, string Name)>> All(CancellationToken token);
    Task<string> Create(string name, CancellationToken token);
    Task<(string Id, string Name)> Read(int index, CancellationToken token);
    Task<NotebookEntity> Read(string id, CancellationToken token);
    Task Update(string id, NotebookEntity entity, CancellationToken token);
    Task Delete(string id, CancellationToken token);
}