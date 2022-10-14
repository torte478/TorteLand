using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Storage;

internal sealed class Entity : IEntity
{
    private readonly ITransaction _transaction;

    public int Key { get; }
    public Note Value { get; private set; }

    public Entity(ITransaction transaction, int key, Note value)
    {
        _transaction = transaction;
        Key = key;
        Value = value;
    }

    public void Update(int weight)
    {
        Value = Value with { Weight = weight };
        _transaction.Update(this);
    }

    public void Update(string text)
    {
        Value = Value with { Text = text };
        _transaction.Update(this);
    }

    public void Delete()
    {
        _transaction.Delete(this);
    }
}