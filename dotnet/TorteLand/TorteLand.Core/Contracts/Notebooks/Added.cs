using System.Collections.Generic;
using SoftwareCraft.Functional;

namespace TorteLand.Core.Contracts.Notebooks;

public sealed record Added(
    IReadOnlyCollection<string> Values,
    bool Exact,
    Maybe<int> Origin,
    Direction Direction)
{
    public Added(IReadOnlyCollection<string> values)
        : this(values, false, Maybe.None<int>(), Direction.After)
    {
    }
}