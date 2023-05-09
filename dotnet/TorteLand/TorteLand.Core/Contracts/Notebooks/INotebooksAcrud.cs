using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Contracts;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebooksAcrud
{
    Task<Page<Unique<string>>> All(Maybe<Pagination> pagination, CancellationToken token);
    Task<int> Create(string name, CancellationToken token);
    Task<Maybe<string>> Read(int index, CancellationToken token);
    Task Update(int index, string name, CancellationToken token);
    Task Delete(int index, CancellationToken token);
}