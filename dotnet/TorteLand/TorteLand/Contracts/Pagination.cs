using SoftwareCraft.Functional;

namespace TorteLand.Contracts;

public record Pagination(
    Maybe<int> Offset,
    Maybe<int> Count);