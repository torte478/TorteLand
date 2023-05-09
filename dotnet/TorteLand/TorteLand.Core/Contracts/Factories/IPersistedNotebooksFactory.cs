using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Contracts.Factories;

// TODO: refactor factories
public interface IPersistedNotebooksFactory
{
    IPersistedNotebook Create(string id);
}