using System.Collections.Generic;

namespace TorteLand;

public record Page<T>(
    IReadOnlyCollection<T> Items,
    int CurrentIndex,
    int TotalItems);