using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

namespace TorteLand.Core.Notebooks;

internal interface IAsyncNotebook<TKey, TValue> : IAsyncEnumerable<(TKey, TValue)>
{
    Task<Either<TKey, Segment<TKey>>> Add(TValue value, Maybe<HalfSegment<TKey>> segment, CancellationToken token);
}