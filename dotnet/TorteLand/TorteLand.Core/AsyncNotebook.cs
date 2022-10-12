using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;

#pragma warning disable CS1998

namespace TorteLand.Core;

internal sealed class AsyncNotebook<TKey, TValue> : IAsyncNotebook<TKey, TValue>
{
    private readonly INotebook<TKey, TValue> _origin;

    public AsyncNotebook(INotebook<TKey, TValue> origin)
    {
        _origin = origin;
    }

    public IAsyncEnumerator<(TKey, TValue)> GetAsyncEnumerator(CancellationToken token)
        => ToAsyncEnumerable().GetAsyncEnumerator(token);

    public Task<Either<TKey, Segment<TKey>>> Add(TValue value, Maybe<HalfSegment<TKey>> segment, CancellationToken token)
        => _origin
           .Add(value, segment)
           ._(Task.FromResult);

    private async IAsyncEnumerable<(TKey, TValue)> ToAsyncEnumerable()
    {
        foreach (var item in _origin)
            yield return item;
    }
}