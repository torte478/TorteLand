using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebooksAcrud
{
    IAsyncEnumerable<Unique<string>> All(CancellationToken token);
    Task<int> Create(string name, CancellationToken token);
    Task Delete(int index, CancellationToken token);
}