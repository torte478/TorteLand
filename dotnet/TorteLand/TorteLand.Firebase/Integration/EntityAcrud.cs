using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using TorteLand.Extensions;

namespace TorteLand.Firebase.Integration;

internal sealed class EntityAcrud : IEntityAcrud
{
    private readonly string _root;
    private readonly FirebaseClient _client;
    private readonly TimeSpan _timeout;

    public EntityAcrud(string root, FirebaseClient client, TimeSpan timeout)
    {
        _root = root;
        _client = client;
        _timeout = timeout;
    }

    public async Task<IReadOnlyCollection<(string Id, string Name)>> All(CancellationToken token)
    {
        var entities = await _client
                       .Child(_root)
                       .OnceAsync<NamedEntity>(_timeout);

        return entities
               .Select(_ => (_.Key, _.Object.Name))
               .ToArray();
    }

    public async Task<string> Create(string name, CancellationToken token)
    {
        var entity = name
                     .Wrap<NamedEntity>()
                     ._(_ => JsonSerializer.Serialize(_));
        
        var created = await _client.Child(_root).PostAsync(entity, timeout: _timeout);
        return created.Key;
    }

    public async Task<(string Id, string Name)> Read(int index, CancellationToken token)
    {
        var entities = await _client
                             .Child(_root)
                             .OnceAsync<NamedEntity>(_timeout);

        return entities
               .ElementAt(index)
               ._(_ => (_.Key, _.Object.Name));
    }

    public async Task<NotebookEntity> Read(string id, CancellationToken token)
    {
        var entity = await _client
                           .Child(_root)
                           .Child(id)
                           .OnceSingleAsync<NotebookEntity>(_timeout);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return entity.Notes is { }
                   ? entity
                   : entity with { Notes = Array.Empty<NoteEntity>() };
    }

    public Task Update(string id, NotebookEntity entity, CancellationToken token)
        => _client
           .Child(_root)
           .Child(id)
           .PutAsync(JsonSerializer.Serialize(entity), _timeout);

    public Task Delete(string id, CancellationToken token)
        => _client
           .Child(_root)
           .Child(id)
           .DeleteAsync(_timeout);
}