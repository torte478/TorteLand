using TorteLand.Core.Contracts;

namespace TorteLand.Core.Storage;

internal sealed class Entity : IEntity
{
    private readonly ITransaction _transaction;
    
    private Note _note;

    public Entity(ITransaction transaction, Note note)
    {
        _transaction = transaction;
        _note = note;
    }

    public void Update(int weight)
    {
        var updated = _note with { Weight = weight };
        _transaction.Update(_note);
        _note = updated;
    }

    public void Delete()
    {
        _transaction.Delete(_note);
    }
}