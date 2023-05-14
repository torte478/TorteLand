namespace TorteLand.Core.Contracts.Notebooks.Models;

public record ResolvedSegment(
    Segment Segment,
    bool IsGreater)
{
    public override string ToString()
        => $"({Segment}, {(IsGreater ? "greater" : "smaller")})";
}