using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

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
    public IAsyncEnumerable<KeyValuePair<int, string>> All(
        int index,
        CancellationToken token)
        => _notebooks
           .All(index, token)
           .Select(_ => new KeyValuePair<int, string>(_.Id, _.Value.Text));

    [HttpPut]
    public int Create() => _notebooks.Create();

    [HttpPost]
    [Route("start_add")]
    public async Task<Models.Either<int, Transaction>> Add(
        int index,
        string value,
        CancellationToken token)
    {
        var result = await _notebooks.Add(
                         index,
                         value,
                         token);

        return result.Match(
            x => new Models.Either<int, Transaction>(x, default),
            x => new Models.Either<int, Transaction>(default, x));
    }

    [HttpPost]
    [Route("continue_add")]
    public async Task<Models.Either<int, Transaction>> Add(
        int index,
        Guid id,
        bool isRight,
        CancellationToken token)
    {
        var result = await _notebooks.Add(index, id, isRight, token);

        return result.Match(
            x => new Models.Either<int, Transaction>(x, default),
            x => new Models.Either<int, Transaction>(default, x));
    }

    [HttpPost]
    [Route("delete")]
    public Task Delete(int index, int key, CancellationToken token)
        => _notebooks.Delete(index, key, token);
}