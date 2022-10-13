using TorteLand.Core.Contracts;

namespace TorteLand.FileStorage.Storages;

internal sealed class TransactionFactory : ITransactionFactory
{
    private readonly IEntityFactory _factory;

    public TransactionFactory(IEntityFactory factory)
    {
        _factory = factory;
    }

    public ITransaction Create(string path)
        => new Transaction(path, _factory);
}