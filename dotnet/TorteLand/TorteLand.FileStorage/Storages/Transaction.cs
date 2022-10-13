using System.Text.Json;
using TorteLand.Core.Contracts;

namespace TorteLand.FileStorage.Storages;

internal sealed class Transaction : ITransaction
{
    private readonly string _path;
    private readonly IEntityFactory _factory;
    private readonly Lazy<List<Note>> _notes;

    public Transaction(string path, IEntityFactory factory)
    {
        _path = path;
        _factory = factory;

        _notes = new Lazy<List<Note>>(Load);
    }

    private List<Note> Load()
    {
        if (!File.Exists(_path))
        {
            File.WriteAllText(_path, string.Empty);
            return new List<Note>();
        }

        var text = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<List<Note>>(text)!;
    }

    public void Create(Note note)
    {
        _notes.Value.Add(note);
    }

    public IEntity ToEntity(Note note)
        => _factory.Create(this, note);

    public void Update(Note note)
    {
        var index = _notes.Value
              .Select((x, i) => (x, i))
              .First(_ => _.x.Text == note.Text)
              .i;

        _notes.Value[index] = _notes.Value[index] with { Weight = note.Weight };
    }

    public Task Save(CancellationToken token)
    {
        if (!_notes.IsValueCreated)
            return Task.CompletedTask;

        var json = JsonSerializer.Serialize(_notes.Value);
        return File.WriteAllTextAsync(_path, json, token);
    }

    public IAsyncEnumerable<Note> All(CancellationToken token)
        => _notes.Value.ToAsyncEnumerable();
}