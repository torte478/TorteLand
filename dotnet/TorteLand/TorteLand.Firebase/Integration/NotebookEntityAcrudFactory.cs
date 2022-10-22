namespace TorteLand.Firebase.Integration;

internal sealed class NotebookEntityAcrudFactory : INotebookEntityAcrudFactory
{
    private readonly string _root;
    private readonly IFirebaseClientFactory _factory;

    public NotebookEntityAcrudFactory(string root, IFirebaseClientFactory factory)
    {
        _root = root;
        _factory = factory;
    }

    public async Task<INotebookEntityAcrud> Create()
    {
        var client = await _factory.Create();
        return new NotebookEntityAcrud(_root, client);
    }
}