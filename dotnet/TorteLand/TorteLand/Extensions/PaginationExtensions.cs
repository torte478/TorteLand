using System.Collections.Generic;
using System.Linq;
using SoftwareCraft.Functional;
using TorteLand.Contracts;

namespace TorteLand.Extensions;

public static class PaginationExtensions
{
    public static Page<T> Paginate<T>(this IEnumerable<T> source, Maybe<Pagination> pagination, int total)
    {
        var config = pagination.Match(
            _ => (
                     count: _.Count.Match(count => count, () => total),
                     offset: _.Offset.Match(offset => offset, () => 0)),
            () => (
                      count: total,
                      offset: 0));

        var notes = source
                    .Skip(config.offset)
                    .Take(config.count)
                    .ToArray();

        return new Page<T>(
            Items: notes,
            CurrentIndex: config.offset,
            TotalItems: total);
    }
}