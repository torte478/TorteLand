using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;

namespace TorteLand.Core.Contracts;

public interface INotebooks<TIndex, TKey, TValue>
{
    IAsyncEnumerable<(TKey, TValue)> All(TIndex index, CancellationToken token);

    TIndex Create();

    Task<Either<TKey, Segment<TKey>>> Add(
        TIndex index,
        TValue value,
        Maybe<HalfSegment<TKey>> segment,
        CancellationToken token);
}