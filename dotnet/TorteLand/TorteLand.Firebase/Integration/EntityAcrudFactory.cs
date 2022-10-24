namespace TorteLand.Firebase.Integration;

internal sealed class EntityAcrudFactory : IEntityAcrudFactory
{
    private readonly string _root;
    private readonly IFirebaseClientFactory _factory;

    public EntityAcrudFactory(string root, IFirebaseClientFactory factory)
    {
        _root = root;
        _factory = factory;
    }

    public async Task<IEntityAcrud> Create()
    {
        var client = await _factory.Create();
        return new EntityAcrud(_root, client);
    }
}