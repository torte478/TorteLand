using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

namespace TorteLand.Core.Notebooks;

// TODO
internal sealed class Notebooks : INotebooks
{
    private readonly IAsyncNotebook _notebook;

    public Notebooks(IFactory factory)
    {
        _notebook = factory.Create();
    }

    public IAsyncEnumerable<Unique<Note>> All(int index, CancellationToken token)
        => _notebook;

    public int Create() => 0;

    public Task<Either<int, Segment>> Add(
        int index,
        string value,
        Maybe<ResolvedSegment> segment,
        CancellationToken token)
        => _notebook.Add(value, segment, token);
}