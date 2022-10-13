using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

#pragma warning disable CS1998

namespace TorteLand.Core.Notebooks;

internal sealed class AsyncNotebook : IAsyncNotebook
{
    private readonly INotebook _origin;

    public AsyncNotebook(INotebook origin)
    {
        _origin = origin;
    }

    public IAsyncEnumerator<Unique<Note>> GetAsyncEnumerator(CancellationToken token)
        => ToAsyncEnumerable().GetAsyncEnumerator(token);

    public Task<Either<int, Segment>> Add(string value, Maybe<ResolvedSegment> segment, CancellationToken token)
        => _origin
           .Add(value, segment)
           ._(Task.FromResult);

    public IAsyncNotebook Clone()
        => _origin
            .Clone()
            ._(_ => new AsyncNotebook(_));

    public Task<Note> ToNote(int key)
        => _origin
           .ToNote(key)
           ._(Task.FromResult);

    private async IAsyncEnumerable<Unique<Note>> ToAsyncEnumerable()
    {
        foreach (var item in _origin)
            yield return item;
    }
}