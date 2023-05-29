using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Notebooks.Models;

namespace TorteLand.Core.Contracts.Storage;

public interface IStorage
{
    Task Save(IReadOnlyCollection<Unique<Note>> notes, CancellationToken token);
    Task<IReadOnlyCollection<Note>> All(CancellationToken token);
}