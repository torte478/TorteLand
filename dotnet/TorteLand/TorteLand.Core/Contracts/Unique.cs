namespace TorteLand.Core.Contracts;

public record Unique<T>(
    int Id,
    T Value);