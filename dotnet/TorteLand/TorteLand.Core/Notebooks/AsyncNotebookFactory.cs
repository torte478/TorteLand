namespace TorteLand.Core.Notebooks;

internal sealed class AsyncNotebookFactory<TValue> : IAsyncNotebookFactory<int, TValue>
{
    public IAsyncNotebook<int, TValue> Create()
        => new AsyncNotebook<int, TValue>(
            origin: new Notebook<TValue>());
}