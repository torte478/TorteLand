﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.App.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class NotebooksController : ControllerBase
{
    private readonly INotebooks _notebooks;

    public NotebooksController(INotebooks notebooks)
    {
        _notebooks = notebooks;
    }

    [HttpGet]
    [Route("All")]
    public IAsyncEnumerable<KeyValuePair<int, string>> All(
        int index,
        CancellationToken token)
        => _notebooks
           .Read(index, token)
           .Select(_ => new KeyValuePair<int, string>(_.Id, _.Value.Text));

    [HttpPost]
    [Route("StartAdd")]
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
    [Route("ContinueAdd")]
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
    [Route("Rename")]
    public Task Rename(int index, int id, string text, CancellationToken token)
        => _notebooks.Rename(index, id, text, token);

    [HttpPost]
    [Route("Delete")]
    public Task Delete(int index, int key, CancellationToken token)
        => _notebooks.Delete(index, key, token);
}