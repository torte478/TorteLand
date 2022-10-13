using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Notebooks;

namespace TorteLand.Core.Contracts;

public interface INotebooks
{
    IAsyncEnumerable<Unique<Note>> All(int index, CancellationToken token);

    int Create();

    Task<Either<int, Segment>> Add(
        int index,
        string value,
        Maybe<ResolvedSegment> segment,
        CancellationToken token);
}