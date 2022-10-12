namespace TorteLand.Core.Notebooks;

internal interface IAsyncNotebookFactory<TKey, TValue>
{
    IAsyncNotebook<TKey, TValue> Create();
}