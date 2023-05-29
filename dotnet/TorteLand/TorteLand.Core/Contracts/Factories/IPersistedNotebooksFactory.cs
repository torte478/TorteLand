using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Contracts.Factories;

public interface IPersistedNotebooksFactory
{
    IPersistedNotebook Create(string id);
}