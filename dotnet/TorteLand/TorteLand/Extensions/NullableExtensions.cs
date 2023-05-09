using SoftwareCraft.Functional;

namespace TorteLand.Extensions;

public static class NullableExtensions
{
    public static Maybe<T> ToMaybe<T>(this T? origin) where T : struct
        => origin.HasValue
               ? Maybe.Some(origin.Value)
               : Maybe.None<T>();
}