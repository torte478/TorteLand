using System.Collections.Generic;
using System.Threading.Tasks;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Contracts.Storage;

public interface IStorage
{
    Task Save(IReadOnlyCollection<Unique<Note>> notes);
    Task DeleteAll();
    Task<IReadOnlyCollection<Note>> All();
}