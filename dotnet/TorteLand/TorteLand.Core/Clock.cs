using System;

namespace TorteLand.Core;

internal sealed class Clock : IClock
{
    public DateTimeOffset ToNow()
        => DateTimeOffset.Now;
}