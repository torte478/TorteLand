using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Contracts.Storage;

public interface IEntityFactory
{
    IEntity Create(ITransaction transaction, Note entity);
}