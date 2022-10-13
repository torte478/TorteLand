using System.Collections.Generic;
using System.Linq;
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

    public IAsyncEnumerable<Unique<Note>> All(CancellationToken token)
        => _origin.ToAsyncEnumerable();

    public Task<Either<int, Segment>> Add(string value, Maybe<ResolvedSegment> segment, CancellationToken token)
        => _origin
           .Add(value, segment)
           ._(Task.FromResult);

    public Task<Note> ToNote(int key, CancellationToken token)
        => _origin
           .ToNote(key)
           ._(Task.FromResult);

    public Task<IAsyncNotebook> Clone(CancellationToken token)
    {
        IAsyncNotebook clone = new AsyncNotebook(_origin.Clone());
        return Task.FromResult(clone);
    }

    public Task<Note> Delete(int key, CancellationToken token)
        => key
           ._(_origin.Delete)
           ._(Task.FromResult);
}