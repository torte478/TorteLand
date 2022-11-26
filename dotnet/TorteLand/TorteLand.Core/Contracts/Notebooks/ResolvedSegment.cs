namespace TorteLand.Core.Contracts.Notebooks;

public record ResolvedSegment(
    Segment Segment,
    bool IsRight) // TODO: IsRight => IsGreater
{
    public override string ToString()
        => $"({Segment}, {(IsRight ? "right" : "left")})";
}