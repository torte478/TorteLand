namespace TorteLand.Core;

public interface INotebookProvider<in T>
{
    IAsyncNotebook<TKey, TValue> Get<TKey, TValue>(T key);
}