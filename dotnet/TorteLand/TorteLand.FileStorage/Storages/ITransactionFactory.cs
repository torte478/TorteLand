using TorteLand.Core.Contracts;

namespace TorteLand.FileStorage.Storages;

internal interface ITransactionFactory
{
    ITransaction Create(string path);
}