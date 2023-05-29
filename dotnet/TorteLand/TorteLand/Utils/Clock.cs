using System;
using TorteLand.Contracts;

namespace TorteLand.Utils;

internal sealed class Clock : IClock
{
    public DateTimeOffset ToNow()
        => DateTimeOffset.Now;
}