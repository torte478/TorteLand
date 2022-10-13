using TorteLand.Core.Contracts.Storage;

namespace TorteLand.FileStorage.Storages;

internal interface IStorageFactory
{
    IStorage Create(string path);
}