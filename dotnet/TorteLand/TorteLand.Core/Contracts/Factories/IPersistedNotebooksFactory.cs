using System.Threading.Tasks;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Contracts.Factories;

// TODO: refactor factories
public interface IPersistedNotebooksFactory
{
    Task<IPersistedNotebook> Create(string id);
}