namespace TorteLand.Core.Contracts;

public record Segment<T>(
    T Begin,
    T Middle,
    T End);