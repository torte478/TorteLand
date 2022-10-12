namespace TorteLand.Core;

public record Segment<T>(
    T Begin,
    T Middle,
    T End);