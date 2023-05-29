using System;
using SoftwareCraft.Functional;

namespace TorteLand.Extensions;

public static class MaybeExtensions
{
    public static T ToSome<T>(this Maybe<T> maybe)
        => maybe.Match(
            _ => _,
            () => throw new Exception("Maybe is None"));
}