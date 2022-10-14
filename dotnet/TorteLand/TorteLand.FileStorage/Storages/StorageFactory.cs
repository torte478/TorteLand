using TorteLand.Core.Contracts.Storage;

namespace TorteLand.FileStorage.Storages;

internal sealed class StorageFactory : IStorageFactory
{
    private readonly ITransactionFactory _factory;

    public StorageFactory(ITransactionFactory factory)
    {
        _factory = factory;
    }

    public IStorage Create(string path) => new Storage(path, _factory);
}