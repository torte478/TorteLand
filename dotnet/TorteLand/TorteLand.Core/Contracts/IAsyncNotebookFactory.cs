using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TorteLand.Core.Contracts;

public interface IAsyncNotebookFactory
{
    Task<IAsyncNotebook> Create(IReadOnlyCollection<Note> notes, CancellationToken token);
}