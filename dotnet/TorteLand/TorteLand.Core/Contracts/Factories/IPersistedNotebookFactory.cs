using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Factories;

public interface IPersistedNotebookFactory
{
    IAsyncNotebook Create(IStorage storage, Either<INotebookFactory, INotebook> origin);
}