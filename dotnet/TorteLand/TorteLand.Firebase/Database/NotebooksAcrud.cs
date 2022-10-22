using Firebase.Database;
using Firebase.Database.Query;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Firebase.Database;

// TODO : add cache
internal sealed class NotebooksAcrud : INotebooksAcrud
{
    private const string Notebooks = "notebooks"; // TODO : to config

    private readonly FirebaseClient _client;

    public NotebooksAcrud(FirebaseClient client)
    {
        _client = client;
    }

    public async Task<Page<Unique<string>>> All(Maybe<Pagination> pagination, CancellationToken token)
    {
        var entities = await Load();

        return entities
               .Select((x, i) => new Unique<string>(i, x.Object.Name))
               .Paginate(pagination, entities.Count);
    }

    public async Task<int> Create(string name, CancellationToken token)
    {
        var created = await _client
              .Child(Notebooks)
              .PostAsync(new NamedEntity(name));

        var entities = await Load();

        return entities
               .Select((x, i) => (x, i))
               .First(_ => _.x.Key == created.Key)
               .i;
    }

    public async Task Delete(int index, CancellationToken token)
    {
        var entities = await Load();
        var key = entities.ElementAt(index).Key;

        await _client
              .Child(Notebooks)
              .Child(key)
              .DeleteAsync();
    }

    public async Task Rename(int index, string name, CancellationToken token)
    {
        var entities = await Load();
        var key = entities.ElementAt(index).Key;

        var notebook = await _client.Child(Notebooks).Child(key).OnceSingleAsync<NotebookEntity>();
        var renamed = notebook with { Name = name };

        await _client
              .Child(Notebooks)
              .Child(key)
              .PutAsync(renamed);
    }

    private Task<IReadOnlyCollection<FirebaseObject<NamedEntity>>> Load()
        => _client
           .Child(Notebooks)
           .OnceAsync<NamedEntity>();
}