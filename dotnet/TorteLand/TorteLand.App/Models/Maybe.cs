namespace TorteLand.App.Models;

public record Maybe<T>(
    bool IsSome,
    T Value);