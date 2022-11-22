using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;

namespace TorteLand.Core.Contracts.Notebooks;

public interface IAsyncNotebook
{
    Task<Page<Unique<Note>>> All(Maybe<Pagination> pagination, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Segment>> Add(IReadOnlyCollection<string> value, Maybe<ResolvedSegment> segment, CancellationToken token);
    Task<Note> ToNote(int key, CancellationToken token);
    Task Update(int key, string name, CancellationToken token);
    Task<Note> Delete(int key, CancellationToken token);
    Task DeleteAll(CancellationToken token);
    Task<Maybe<string>> Read(int key, CancellationToken token);
}