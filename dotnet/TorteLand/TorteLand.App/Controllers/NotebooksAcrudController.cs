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
    private readonly INotebooksAcrud _acrud;

    public NotebooksAcrudController(INotebooksAcrud acrud)
    {
        _acrud = acrud;
    }

    [HttpGet]
    [Route("All")]
    public async Task<Page<Unique<string>>> All(int? count, int? offset, CancellationToken token)
    {
        var pagination = count is { } || offset is { }
                             ? new Pagination(
                                     Offset: offset.ToMaybe(),
                                     Count: count.ToMaybe())
                                 ._(Maybe.Some)
                             : Maybe.None<Pagination>();

        return await _acrud.All(pagination, token);
    }

    [HttpPost]
    [Route("Create")]
    public Task<int> Create(string name, CancellationToken token)
        => _acrud.Create(name, token);

    [HttpPost]
    [Route("Rename")]
    public Task Rename(int index, string name, CancellationToken token)
        => _acrud.Rename(index, name, token);

    [HttpPost]
    [Route("Delete")]
    public Task Delete(int index, CancellationToken token)
        => _acrud.Delete(index, token);
}