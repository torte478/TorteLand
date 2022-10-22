using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Firebase.Database;

internal sealed class NotebooksAcrudFactory : INotebooksAcrudFactory
{
    private readonly IFirebaseClientFactory _factory;

    public NotebooksAcrudFactory(IFirebaseClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<INotebooksAcrud> Create()
    {
        var client = await _factory.Create();
        return new NotebooksAcrud(client);
    }
}