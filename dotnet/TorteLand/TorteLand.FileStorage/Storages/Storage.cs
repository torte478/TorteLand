using TorteLand.Core.Contracts.Storage;

namespace TorteLand.FileStorage.Storages;

internal sealed class Storage : IStorage
{
    private readonly string _path;
    private readonly ITransactionFactory _factory;

    public Storage(string path, ITransactionFactory factory)
    {
        _path = path;
        _factory = factory;
    }

    public ITransaction StartTransaction()
        => _factory.Create(_path);
}