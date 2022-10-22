using Firebase.Database;
using Firebase.Database.Query;

namespace TorteLand.Firebase.Integration;

internal sealed class EntityAcrud : IEntityAcrud
{
    private readonly string _root;
    private readonly FirebaseClient _client;

    public EntityAcrud(string root, FirebaseClient client)
    {
        _root = root;
        _client = client;
    }

    public async Task<IReadOnlyCollection<(string Id, string Name)>> All()
    {
        var entities = await _client
                       .Child(_root)
                       .OnceAsync<NamedEntity>();

        return entities
               .Select(_ => (_.Key, _.Object.Name))
               .ToArray();
    }

    public async Task<string> Create(string name)
    {
        var entity = new NamedEntity(name);
        var created = await _client.Child(_root).PostAsync(entity);
        return created.Key;
    }

    public async Task<(string Id, string Name)> Read(int index)
    {
        var entities = await _client
                             .Child(_root)
                             .OnceAsync<NamedEntity>();

        return entities
               .ElementAt(index)
               ._(_ => (_.Key, _.Object.Name));
    }

    public async Task<NotebookEntity> Read(string id)
    {
        var entity = await _client
                           .Child(_root)
                           .Child(id)
                           .OnceSingleAsync<NotebookEntity>();

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return entity.Notes is { }
                   ? entity
                   : entity with { Notes = Array.Empty<string>() };
    }

    public Task Update(string id, NotebookEntity entity)
        => _client
           .Child(_root)
           .Child(id)
           .PutAsync(entity);

    public Task Delete(string id)
        => _client
           .Child(_root)
           .Child(id)
           .DeleteAsync();
}