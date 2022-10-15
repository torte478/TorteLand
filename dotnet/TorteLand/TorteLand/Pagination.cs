using SoftwareCraft.Functional;

namespace TorteLand;

public record Pagination(
    Maybe<int> Offset,
    Maybe<int> Count);