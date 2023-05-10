using System;
using Microsoft.Extensions.Options;

namespace TorteLand.Firebase.Integration;

internal sealed class EntityAcrudFactory : IEntityAcrudFactory
{
    private readonly string _root;
    private readonly IFirebaseClientFactory _factory;
    private readonly TimeSpan _timeout;

    public EntityAcrudFactory(IOptions<FirebaseSettings> settings, IFirebaseClientFactory factory)
    {
        _root = settings.Value.Root;
        _timeout = settings.Value.Timeout;
        _factory = factory;
    }

    public IEntityAcrud Create()
    {
        var client = _factory.Create();
        return new EntityAcrud(_root, client, _timeout);
    }
}