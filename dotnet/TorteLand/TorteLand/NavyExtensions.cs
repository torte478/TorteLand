namespace TorteLand;

public static class NavyExtensions
{
    public static TOut _<TIn, TOut>(this TIn x, Func<TIn, TOut> f)
        => f(x);

    public static void _<TIn>(this TIn x, Action<TIn> f)
        => f(x);
}