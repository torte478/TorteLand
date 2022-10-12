using System.Collections.Generic;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

namespace TorteLand.Core.Notebooks;

internal interface INotebook<TKey, TValue> : IEnumerable<(TKey, TValue)>
{
    Either<TKey, Segment<TKey>> Add(TValue value, Maybe<HalfSegment<TKey>> segment);
}