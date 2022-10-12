using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

namespace TorteLand.Core.Notebooks;

internal sealed class Notebooks<TIndex, TKey, TValue>
    : INotebooks<TIndex, TKey, TValue> where TIndex : notnull
{
    private readonly Dictionary<TIndex, IAsyncNotebook<TKey, TValue>> _notebooks;

    private readonly IAsyncNotebookFactory<TKey, TValue> _factory;
    private readonly IKeyGenerator<TIndex> _generator;

    public Notebooks(IAsyncNotebookFactory<TKey, TValue> factory, IKeyGenerator<TIndex> generator)
    {
        _factory = factory;
        _generator = generator;

        _notebooks = new Dictionary<TIndex, IAsyncNotebook<TKey, TValue>>();
    }

    public IAsyncEnumerable<(TKey, TValue)> All(TIndex index, CancellationToken token)
        => _notebooks[index];

    public TIndex Create()
    {
        var key = _generator.Next();
        _notebooks.Add(key, _factory.Create());
        return key;
    }

    public Task<Either<TKey, Segment<TKey>>> Add(
        TIndex index,
        TValue value,
        Maybe<HalfSegment<TKey>> segment,
        CancellationToken token)
        => _notebooks[index].Add(value, segment, token);
}