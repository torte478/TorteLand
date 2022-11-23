using System.Threading.Tasks;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Firebase.Database;

internal interface INotebookFactory
{
    Task<IPersistedNotebook> Create(string id);
}