using System.Reflection;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.FileStorage.Storages;

internal sealed class FileNotebooks : INotebooks
{
    private readonly Dictionary<int, (string Path, IQuestionableNotebook Notebook)> _cache;

    private readonly string _path;
    private readonly INotebookFactory _factory;

    private int _next;

    public FileNotebooks(string path, INotebookFactory factory)
    {
        _factory = factory;

        _path = BuildPath(path);
        _cache = BuildCache(path, factory);
    }

    public IAsyncEnumerable<Unique<Note>> All(int index, CancellationToken token)
        => _cache[index].Notebook.All(token);

    public Task<int> Create(string name, CancellationToken token)
    {
        var path = Path.Combine(_path, $"{name}.json");
        if (File.Exists(path))
            throw new Exception($"File exists: {path}");

        var notebook = _factory.Create(path);
        var key = _next++;
        _cache.Add(key, (path, notebook));

        return Task.FromResult(key);
    }

    public Task<Either<int, Question>> Add(int index, string value, CancellationToken token)
        => _cache[index].Notebook.Add(value, token);

    public Task<Either<int, Question>> Add(int index, Guid id, bool isRight, CancellationToken token)
        => _cache[index].Notebook.Add(id, isRight, token);

    public Task Delete(int index, int key, CancellationToken token)
        => _cache[index].Notebook.Delete(key, token);

    private Dictionary<int, (string Path, IQuestionableNotebook Notebook)> BuildCache(string path, INotebookFactory factory)
        => path
           ._(Directory.EnumerateFiles)
           .Select((_, i) => (i, (Path.GetFileName(_), factory.Create(_))))
           .ToDictionary(
               _ => _.i,
               _ => _.Item2);

    private static string BuildPath(string path)
    {
        var name = Assembly.GetExecutingAssembly().FullName!;
        var full = Assembly.GetExecutingAssembly().Location[..^name.Length];
        var result = Path.Combine(full, path);

        if (!Directory.Exists(result))
            Directory.CreateDirectory(result);

        return result;
    }
}