using System.Threading.Tasks;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebooksAcrudFactory
{
    Task<INotebooksAcrud> Create();
}