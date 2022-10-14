using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.App.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class NotebooksAcrudController : ControllerBase
{
    private readonly INotebooksAcrud _notebooksAcrud;

    public NotebooksAcrudController(INotebooksAcrud notebooksAcrud)
    {
        _notebooksAcrud = notebooksAcrud;
    }
    
    [HttpGet]
    [Route("all")]
    public IAsyncEnumerable<Unique<string>> All(CancellationToken token)
        => _notebooksAcrud.All(token);

    [HttpPost]
    [Route("create")]
    public Task<int> Create(string name, CancellationToken token)
        => _notebooksAcrud.Create(name, token);

    [HttpPost]
    [Route("delete")]
    public Task Delete(int index, CancellationToken token)
        => _notebooksAcrud.Delete(index, token);
}