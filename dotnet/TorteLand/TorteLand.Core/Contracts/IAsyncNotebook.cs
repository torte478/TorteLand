using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;

namespace TorteLand.Core.Contracts;

public interface IAsyncNotebook
{
    IAsyncEnumerable<Unique<Note>> All(CancellationToken token);
    Task<Either<int, Segment>> Add(string value, Maybe<ResolvedSegment> segment, CancellationToken token);
    Task<Note> ToNote(int key, CancellationToken token);
    Task<IAsyncNotebook> Clone(CancellationToken token);
    Task<Note> Delete(int key, CancellationToken token);
}