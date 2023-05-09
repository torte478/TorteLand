using System.Collections.Generic;

namespace TorteLand.Contracts;

public record Page<T>(
    IReadOnlyCollection<T> Items,
    int CurrentIndex,
    int TotalItems);