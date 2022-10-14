using System;

namespace TorteLand.Bot.Utils;

internal sealed class Clock : IClock
{
    public DateTime Now() => DateTime.UtcNow;
}