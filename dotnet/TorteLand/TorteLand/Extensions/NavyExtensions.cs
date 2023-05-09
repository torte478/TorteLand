using System;

namespace TorteLand.Extensions;

public static class NavyExtensions
{
    public static TOut _<TIn, TOut>(this TIn x, Func<TIn, TOut> f)
        => f(x);

    public static TOut _<T1, T2, TOut>(this T1 x, Func<T1, T2, TOut> f, T2 y)
        => f(x, y);

    public static TOut _<T1, T2, T3, TOut>(this T1 x, Func<T1, T2, T3, TOut> f, T2 y, T3 z)
        => f(x, y, z);

    public static void _<TIn>(this TIn x, Action<TIn> f)
        => f(x);

    public static T _<T>(this object origin)
        where T : class
    {
        var instance = Activator.CreateInstance(typeof(T), origin);
        return (instance as T)!;
    }
}