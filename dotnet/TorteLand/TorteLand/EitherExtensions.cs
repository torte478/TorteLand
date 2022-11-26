using System;
using SoftwareCraft.Functional;

namespace TorteLand;

public static class EitherExtensions
{
    public static TLeft ToLeft<TLeft, TRight>(this Either<TLeft, TRight> either)
        => either.Match(
            _ => _,
            _ => throw new Exception($"Either is right: {_}"));
    
    public static TRight ToRight<TLeft, TRight>(this Either<TLeft, TRight> either)
        => either.Match(
            _ => throw new Exception($"Either is left: {_}"),
            _ => _);
}