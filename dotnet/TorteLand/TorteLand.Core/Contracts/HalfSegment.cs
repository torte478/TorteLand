namespace TorteLand.Core.Contracts;

public record HalfSegment<T>(
    Segment<T> Segment,
    bool IsRight);