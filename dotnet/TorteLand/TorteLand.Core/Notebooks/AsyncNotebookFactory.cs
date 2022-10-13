using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

namespace TorteLand.Core.Notebooks;

internal sealed class AsyncNotebookFactory : IAsyncNotebookFactory
{
    public Task<IAsyncNotebook> Create(IReadOnlyCollection<Note> notes, CancellationToken token)
        => notes
           .OrderBy(x => x.Weight)
           .Select(x => x.Text)
           .ToList()
           ._(Maybe.Some)
           ._(_ => new Notebook(_))
           ._(_ => new AsyncNotebook(_) as IAsyncNotebook)
           ._(Task.FromResult);
}