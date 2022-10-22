using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Firebase.Integration;

namespace TorteLand.Firebase;

// TODO : remove after using
internal sealed class FileStorageToFirebaseMigration : IHostedService
{
    private readonly string _path;
    private readonly AsyncLazy<IEntityAcrud> _acrud;

    public FileStorageToFirebaseMigration(string path, IEntityAcrudFactory factory)
    {
        _path = path;
        _acrud = new AsyncLazy<IEntityAcrud>(factory.Create);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var acrud = await _acrud;

        var existing = await acrud.All();

        var directory = Assembly
                        .GetExecutingAssembly()
                        .Location
                        ._(Path.GetDirectoryName)
                        !._(Path.Combine, _path);

        foreach (var file in Directory.EnumerateFiles(directory))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (existing.Any(_ => _.Name == name))
                continue;

            var text = await File.ReadAllTextAsync(file, cancellationToken);
            var notes = (JsonSerializer.Deserialize<Note[]>(text) ?? Array.Empty<Note>())
                        .OrderByDescending(_ => _.Weight)
                        .Select(_ => _.Text)
                        .ToArray();

            var id = await acrud.Create(name);
            await acrud.Update(id, new NotebookEntity(name, notes));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}