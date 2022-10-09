using System.Reflection;
using TorteLand.Core;

#pragma warning disable CS8425
#pragma warning disable CS1998

namespace TorteLand.FileStorage;

internal sealed class FileStorage : IAcrud<int, string>
{
    private const string Extension = ".json";

    private readonly Lazy<string> _root;
    private readonly List<string> _files;

    public FileStorage(string root)
    {
        _root = new Lazy<string>(() => Initialize(root));
        _files = new List<string>();
    }

    public async IAsyncEnumerable<(int Key, string Value)> All(CancellationToken token)
    {
        Reload();

        for (var i = 0; i < _files.Count; ++i)
            yield return (i, Shrink(_files[i]));
    }

    public Task<(int Key, string Value)> Create(string value, CancellationToken token)
    {
        var name = $"{value}{Extension}";

        var exists = _files.Any(x => string.Equals(x, name, StringComparison.InvariantCultureIgnoreCase));
        if (exists)
            throw new Exception($"file '{value}' exists");

        File.Create(Path.Combine(_root.Value, name));

        Reload();

        var index = Enumerable.Range(0, _files.Count).First(i => _files[i] == name);
        return Task.FromResult((index, Shrink(name)));
    }

    public Task<string> Read(int key, CancellationToken token)
    {
        if (_files.Count == 0)
            throw new Exception("Folder is empty. Reload maybe?");

        var name = Shrink(_files[key]);
        return Task.FromResult(name);
    }

    public Task<string> Update(int key, string value, CancellationToken token)
    {
        var updated = $"{value}{Extension}";

        var name = _files[key];
        File.Move(Path.Combine(_root.Value, name), Path.Combine(_root.Value, updated));

        Reload();
        return Task.FromResult(updated);
    }

    public Task<string> Delete(int key, CancellationToken token)
    {
        var name = _files[key];
        File.Delete(name);
        Reload();
        return Task.FromResult(name);
    }

    private void Reload()
    {
        _files.Clear();

        Directory
            .GetFiles(_root.Value)
            .Select(Path.GetFileName)
            ._(_files.AddRange!);
    }

    private static string Initialize(string root)
    {
        var path = Assembly
            .GetExecutingAssembly()
            .Location
            ._(_ => _[..^Path.GetFileName(_).Length])
            ._(_ => Path.Combine(_, root));

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    private static string Shrink(string name)
        => name[..^Extension.Length];
}