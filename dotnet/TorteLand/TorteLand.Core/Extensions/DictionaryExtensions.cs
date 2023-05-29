using System.Collections.Generic;
using System.Linq;

namespace TorteLand.Core.Extensions;

internal static class ImmutableCollectionExtensions
{
    public static Dictionary<TKey, TValue> AddImmutable<TKey, TValue>(
        this Dictionary<TKey, TValue> origin,
        TKey key,
        TValue value)
        where TKey : notnull
    {
        var updated = origin.ToDictionary(_ => _.Key, _ => _.Value);
        updated.Add(key, value);
        return updated;
    }
    
    public static Dictionary<TKey, TValue> SetImmutable<TKey, TValue>(
        this Dictionary<TKey, TValue> origin,
        TKey key,
        TValue value)
        where TKey : notnull
    {
        var updated = origin.ToDictionary(_ => _.Key, _ => _.Value);
        updated[key] = value;
        return updated;
    }
}