using TorteLand.Core.Contracts.Storage;

namespace TorteLand.FileStorage.Storages;

internal interface ITransactionFactory
{
    ITransaction Create(string path);
}