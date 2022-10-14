namespace TorteLand.Core.Contracts.Storage;

public interface IStorage
{
    ITransaction StartTransaction();
}