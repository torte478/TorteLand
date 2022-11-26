using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Factories;

public interface IPersistedNotebookFactory
{
    IPersistedNotebook Create(
        IStorage storage, 
        Either<IQuestionableNotebookFactory, IQuestionableNotebook> origin);
}