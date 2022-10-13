namespace TorteLand.Core.Contracts;

public interface IEntityFactory
{
    IEntity Create(ITransaction transaction, Note entity);
}