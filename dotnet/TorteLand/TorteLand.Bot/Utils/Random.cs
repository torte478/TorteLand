using System;

namespace TorteLand.Bot.Utils;

internal sealed class Random : IRandom
{
    private readonly System.Random _random;

    public Random()
    {
        _random = new System.Random(DateTime.Now.Millisecond);
    }

    public int Next(int max) => _random.Next(max);
}