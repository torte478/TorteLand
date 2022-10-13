using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Storage;

internal sealed class EntityFactory : IEntityFactory
{
    public IEntity Create(ITransaction transaction, Note entity)
        => new Entity(transaction, entity);
}