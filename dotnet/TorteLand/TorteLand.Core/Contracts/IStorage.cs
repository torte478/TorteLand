namespace TorteLand.Core.Contracts;

public interface IStorage
{
    ITransaction StartTransaction();
}