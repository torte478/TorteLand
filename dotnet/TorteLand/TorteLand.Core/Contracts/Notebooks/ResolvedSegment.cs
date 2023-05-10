namespace TorteLand.Core.Contracts.Notebooks;

public record ResolvedSegment(
    Segment Segment,
    bool IsGreater)
{
    public override string ToString()
        => $"({Segment}, {(IsGreater ? "greater" : "smaller")})";
}