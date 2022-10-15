﻿using System.Reflection;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.FileStorage.Storages;

internal sealed class FileNotebooks : INotebooksAcrud, INotebooks
{
    private readonly Dictionary<int, (string Path, IQuestionableNotebook Notebook)> _cache;

    private readonly string _path;
    private readonly INotebookFactory _factory;

    public FileNotebooks(string path, INotebookFactory factory)
    {
        _factory = factory;

        _path = BuildPath(path);
        _cache = BuildCache(_path, factory);
    }

    public Task<Page<Unique<Note>>> Read(int index, Maybe<Pagination> pagination, CancellationToken token)
        => _cache[index].Notebook.All(pagination, token);

    public Task<Page<Unique<string>>> All(Maybe<Pagination> pagination, CancellationToken token)
        => _cache
           .Select(_ => new Unique<string>(_.Key, Path.GetFileNameWithoutExtension(_.Value.Path)))
           .Paginate(pagination, _cache.Count)
           ._(Task.FromResult);

    public Task<int> Create(string name, CancellationToken token)
    {
        var path = Path.Combine(_path, $"{name}.json");
        if (File.Exists(path))
            throw new Exception($"File exists: {path}");

        var notebook = _factory.Create(path);
        var key = _cache.Count > 0
                      ? _cache.Keys.Max() + 1
                      : 0;
        _cache.Add(key, (path, notebook));

        return Task.FromResult(key);
    }

    public Task<Either<IReadOnlyCollection<int>, Question>> Add(
        int index,
        IReadOnlyCollection<string> values,
        CancellationToken token)
        => _cache[index].Notebook.Add(values, token);

    public Task<Either<IReadOnlyCollection<int>, Question>> Add(
        int index,
        Guid id,
        bool isRight,
        CancellationToken token)
        => _cache[index].Notebook.Add(id, isRight, token);

    public Task Rename(int index, int id, string text, CancellationToken token)
        => _cache[index].Notebook.Rename(id, text, token);

    public async Task Delete(int index, int key, CancellationToken token)
    {
        await _cache[index].Notebook.Delete(key, token);
        _cache.Remove(index);
    }

    public async Task Delete(int index, CancellationToken token)
    {
        await _cache[index].Notebook.DeleteAll(token);
        _cache.Remove(index);
    }

    public Task Rename(int index, string name, CancellationToken token)
    {
        var old = _cache[index].Path;
        var target = Path.Combine(_path, $"{name}.json");
        File.Copy(old, target);
        _cache[index] = (target, _factory.Create(target));
        File.Delete(old);

        return Task.CompletedTask;
    }

    private static Dictionary<int, (string Path, IQuestionableNotebook Notebook)> BuildCache(
        string path,
        INotebookFactory factory)
        => path
           ._(Directory.EnumerateFiles)
           .Select((file, i) => (i, (file, factory.Create(file))))
           .ToDictionary(
               _ => _.i,
               _ => _.Item2);

    private static string BuildPath(string path)
    {
        var full = Assembly
                   .GetExecutingAssembly()
                   .Location
                   ._(Path.GetDirectoryName)
                   !._(Path.Combine, path);

        if (!Directory.Exists(full))
            Directory.CreateDirectory(full);

        return full;
    }
}