namespace TorteLand.Core.Contracts.Notebooks;

public record ResolvedSegment(
    Segment Segment,
    bool IsRight) // TODOv2: IsRight => IsGreater
{
    public override string ToString()
        => $"({Segment}, {(IsRight ? "right" : "left")})";
}