namespace TorteLand.App.Models;

public record Either<TLeft, TRight>(
    TLeft? Left,
    TRight? Right);