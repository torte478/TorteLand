using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;

namespace TorteLand.Core.Contracts.Notebooks;

public interface IAsyncNotebook
{
    IAsyncEnumerable<Unique<Note>> All(CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Segment>> Add(IReadOnlyCollection<string> value, Maybe<ResolvedSegment> segment, CancellationToken token);
    Task<Note> ToNote(int key, CancellationToken token);
    Task Rename(int key, string text, CancellationToken token);
    Task<Note> Delete(int key, CancellationToken token);
    Task DeleteAll(CancellationToken token);
}