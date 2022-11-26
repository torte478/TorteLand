using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace TorteLand.Firebase.Integration;

internal sealed class EntityAcrudFactory : IEntityAcrudFactory
{
    private readonly string _root;
    private readonly IFirebaseClientFactory _factory;

    public EntityAcrudFactory(IOptions<FirebaseSettings> settings, IFirebaseClientFactory factory)
    {
        _root = settings.Value.Root;
        _factory = factory;
    }

    public async Task<IEntityAcrud> Create()
    {
        var client = await _factory.Create();
        return new EntityAcrud(_root, client);
    }
}