using System.Threading;
using System.Threading.Tasks;

namespace TorteLand.Core.Contracts;

public interface ITransaction
{
    void Create(Note note);
    IEntity ToEntity(Note note);
    void Update(Note note);
    Task Save(CancellationToken token);
}