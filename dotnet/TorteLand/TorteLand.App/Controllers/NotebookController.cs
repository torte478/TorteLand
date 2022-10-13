using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.App.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class NotebookController : ControllerBase
{
    private readonly INotebooks _notebooks;

    public NotebookController(INotebooks notebooks)
    {
        _notebooks = notebooks;
    }

    [HttpGet]
    [Route("read_notebook")]
    public IAsyncEnumerable<KeyValuePair<int, string>> ReadNotebook(
        int index,
        CancellationToken token)
        => _notebooks
           .Read(index, token)
           .Select(_ => new KeyValuePair<int, string>(_.Id, _.Value.Text));

    [HttpGet]
    [Route("all_notebooks")]
    public IAsyncEnumerable<Unique<string>> AllNotebooks(CancellationToken token)
        => _notebooks.All(token);

    [HttpPost]
    [Route("create_notebook")]
    public Task<int> Create(string name, CancellationToken token)
        => _notebooks.Create(name, token);

    [HttpPost]
    [Route("start_add")]
    public async Task<Models.Either<int, Question>> Add(
        int index,
        string value,
        CancellationToken token)
    {
        var result = await _notebooks.Add(
                         index,
                         value,
                         token);

        return result.Match(
            x => new Models.Either<int, Question>(x, default),
            x => new Models.Either<int, Question>(default, x));
    }

    [HttpPost]
    [Route("continue_add")]
    public async Task<Models.Either<int, Question>> Add(
        int index,
        Guid id,
        bool isRight,
        CancellationToken token)
    {
        var result = await _notebooks.Add(index, id, isRight, token);

        return result.Match(
            x => new Models.Either<int, Question>(x, default),
            x => new Models.Either<int, Question>(default, x));
    }

    [HttpPost]
    [Route("delete")]
    public Task Delete(int index, int key, CancellationToken token)
        => _notebooks.Delete(index, key, token);
}