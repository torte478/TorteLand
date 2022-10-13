using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

namespace TorteLand.Core.Notebooks;

internal interface IAsyncNotebook : IAsyncEnumerable<Unique<Note>>
{
    Task<Either<int, Segment>> Add(string value, Maybe<ResolvedSegment> segment, CancellationToken token);
    IAsyncNotebook Clone();
    Task<Note> ToNote(int key);
}