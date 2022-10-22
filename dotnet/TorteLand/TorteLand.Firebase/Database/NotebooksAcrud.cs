using Firebase.Database;
using Firebase.Database.Query;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Firebase.Database;

internal sealed class NotebooksAcrud : INotebooksAcrud
{
    private const string Notebooks = "notebooks";

    private readonly IFirebaseClientFactory _factory;
    private FirebaseClient? _client;

    public NotebooksAcrud(IFirebaseClientFactory factory)
    {
        _factory = factory;
    }

    public Task<Page<Unique<string>>> All(Maybe<Pagination> pagination, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<int> Create(string name, CancellationToken token)
    {
        var client = await GetClient();
        await _client!.Child(Notebooks).PostAsync(new TempNotebook(name, new TempNote[] { new TempNote("NoteName", 42)}));
        return 0;
    }

    public Task Delete(int index, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task Rename(int index, string name, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    private async ValueTask<FirebaseClient> GetClient()
        => _client ??= await _factory.Create();
}