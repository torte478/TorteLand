using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Contracts.Storage;

public interface IEntity
{
    int Key { get; }
    Note Value { get; }

    void Update(int weight);
    void Update(string text);
    void Delete();
}