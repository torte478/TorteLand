using System.Threading;
using System.Threading.Tasks;

namespace TorteLand.Core.Contracts;

public interface IInitialization
{
    string Name { get; }
    
    Task Initialize(CancellationToken token);
}