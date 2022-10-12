using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;

namespace TorteLand.Core;

public interface IAsyncNotebook<TKey, TValue> : IAsyncEnumerable<(TKey, TValue)>
{
    Task<Either<TKey, Segment<TKey>>> Add(TValue value, Maybe<HalfSegment<TKey>> segment, CancellationToken token);
}