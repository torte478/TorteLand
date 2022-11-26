using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Storage;

internal sealed class PersistedNotebookFactory : IPersistedNotebookFactory
{
    public IPersistedNotebook Create(IStorage storage, Either<IQuestionableNotebookFactory, IQuestionableNotebook> origin)
        => new PersistedNotebook(storage, origin);
}