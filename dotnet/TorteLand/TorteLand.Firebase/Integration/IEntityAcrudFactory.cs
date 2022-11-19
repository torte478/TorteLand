using System.Threading.Tasks;

namespace TorteLand.Firebase.Integration;

internal interface IEntityAcrudFactory
{
    Task<IEntityAcrud> Create();
}