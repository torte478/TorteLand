using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.App.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class NotebooksAcrudController : ControllerBase
{
    private readonly INotebooksAcrudFactory _factory;

    private INotebooksAcrud? _notebooksAcrud;

    public NotebooksAcrudController(INotebooksAcrudFactory factory)
    {
        _factory = factory;
    }

    [HttpGet]
    [Route("All")]
    public async Task<Page<Unique<string>>> All(int? count, int? offset, CancellationToken token)
    {
        var acrud = await GetAcrud();

        var pagination = count is { } || offset is { }
                             ? new Pagination(
                                     Offset: offset.ToMaybe(),
                                     Count: count.ToMaybe())
                                 ._(Maybe.Some)
                             : Maybe.None<Pagination>();

        return await acrud.All(pagination, token);
    }

    [HttpPost]
    [Route("Create")]
    public async Task<int> Create(string name, CancellationToken token)
    {
        var acrud = await GetAcrud();
        return await acrud.Create(name, token);
    }

    [HttpPost]
    [Route("Rename")]
    public Task Rename(int index, string name, CancellationToken token)
        => GetAcrud()
            .ContinueWith(
                _ => _.Result.Rename(index, name, token),
                token);

    [HttpPost]
    [Route("Delete")]
    public Task Delete(int index, CancellationToken token)
        => GetAcrud()
            .ContinueWith(
                _ => _.Result.Delete(index, token),
                token);

    private async Task<INotebooksAcrud> GetAcrud()
        => _notebooksAcrud ??= await _factory.Create();
}