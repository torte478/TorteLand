namespace TorteLand.Extensions;

public static class ArrayExtensions
{
    public static T[] AsArray<T>(this T item)
        => new[] { item };
}