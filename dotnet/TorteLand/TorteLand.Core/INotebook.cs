using System.Collections.Generic;
using SoftwareCraft.Functional;

namespace TorteLand.Core;

public interface INotebook<TKey, TValue> : IEnumerable<(TKey, TValue)>
{
    Either<TKey, Segment<TKey>> Add(TValue value, Maybe<HalfSegment<TKey>> segment);
}