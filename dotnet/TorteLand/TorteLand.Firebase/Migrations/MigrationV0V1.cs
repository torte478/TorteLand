using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;
using TorteLand.Core.Contracts;
using TorteLand.Extensions;
using TorteLand.Firebase.Integration;

namespace TorteLand.Firebase.Migrations;

internal sealed class MigrationV0V1 : IInitialization
{
    private const string Version = "version";
    private const string Notebooks = "notebooks";
    private const string Notes = "Notes";

    private const int Next = 1;
    
    private readonly FirebaseClient _client;
    private readonly TimeSpan _timeout;
    
    public string Name => nameof(MigrationV0V1);

    public MigrationV0V1(IFirebaseClientFactory factory, IOptions<FirebaseSettings> settings)
    {
        _client = factory.Create();
        _timeout = settings.Value.Timeout;
    }

    public async Task Initialize(CancellationToken token)
    {
        var version = (await _client.Child(Version).OnceAsync<int>(_timeout))
            .FirstOrDefault();
        
        if (version is not null)
            return;

        await Migrate();

        await _client
              .Child(Version)
              .PostAsync(Next);
    }

    private async Task Migrate()
    {
        var ids = await _client.Child(Notebooks).OnceAsync<NamedEntity>(_timeout);
        
        foreach (var id in ids)
        {
            var query = _client
                        .Child(Notebooks)
                        .Child(id.Key)
                        .Child(Notes);

            var notes = await query.OnceSingleAsync<string[]>(_timeout);

            var updated = notes.Select(ToNoteEntity);

            await query.PutAsync(updated);
        }
    }

    private static NoteEntity ToNoteEntity(string text)
    {
        var tokens = text.Split(' ');
        var suffix = tokens[^1];
        
        if (suffix.Any(c => c is not ('+' or '!')))
            return new NoteEntity(text, 0);

        return new NoteEntity(
            Text: tokens.Take(tokens.Length - 1)._(_ => string.Join(' ', _)),
            Pluses: suffix.Count(c => c == '+'));
    }
}