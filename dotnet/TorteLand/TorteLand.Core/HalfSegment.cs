namespace TorteLand.Core;

public record HalfSegment<T>(
    Segment<T> Segment,
    bool IsRight);