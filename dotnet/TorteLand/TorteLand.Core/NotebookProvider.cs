namespace TorteLand.Core;

internal sealed class NotebookProvider<T> : INotebookProvider<T>
{
    public IAsyncNotebook<TKey, TValue> Get<TKey, TValue>(T key)
    {
        throw new System.NotImplementedException();
    }
}