using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftwareCraft.Functional;
using TorteLand.App.Extensions;
using TorteLand.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Extensions;
using AddResult = TorteLand.App.Models.Either<
    System.Collections.Generic.IReadOnlyCollection<int>,
    TorteLand.Core.Contracts.Storage.Question>;

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

    [HttpGet("[action]")]

    public async Task<Page<KeyValuePair<int, string>>> All(
        int index,
        int? count,
        int? offset,
        CancellationToken token)
    {
        var pagination = count is { } || offset is { }
                             ? new Pagination(
                                     Offset: offset.ToMaybe(),
                                     Count: count.ToMaybe())
                                 ._(Maybe.Some)
                             : Maybe.None<Pagination>();

        var page = await _notebooks.All(index, pagination, token);

        return new Page<KeyValuePair<int, string>>(
            Items: page.Items.Select(_ => new KeyValuePair<int, string>(_.Id, _.Value.Text)).ToArray(),
            CurrentIndex: page.CurrentIndex,
            TotalItems: page.TotalItems);
    }


    [HttpPost("[action]")]
    public Task<AddResult> StartAdd(
        int index,
        IReadOnlyCollection<string> values,
        CancellationToken token)
        => _notebooks.Add(index, values, token).ToModel();

    [HttpPost("[action]")]
    public Task<AddResult> ContinueAdd(
        int index,
        Guid id,
        bool isRight,
        CancellationToken token)
        => _notebooks.Add(index, id, isRight, token).ToModel();

    [HttpGet("[action]")]
    public async Task<Models.Maybe<string>> Read(int index, int id, CancellationToken token)
    {
        var note = await _notebooks.Read(index, id, token);

        return note.Match(
            _ => new Models.Maybe<string>(true, _),
            () => new Models.Maybe<string>(false, string.Empty));
    }

    [HttpPost("[action]")]
    public Task Update(int index, int id, string name, CancellationToken token)
        => _notebooks.Update(index, id, name, token);

    [HttpPost("[action]")]
    public Task Delete(int index, int id, CancellationToken token)
        => _notebooks.Delete(index, id, token);

    [HttpPost("[action]")]
    public Task<Models.Either<byte, int>> Increment(int index, int id, CancellationToken token)
        => _notebooks.Increment(index, id, token).ToModel();
}