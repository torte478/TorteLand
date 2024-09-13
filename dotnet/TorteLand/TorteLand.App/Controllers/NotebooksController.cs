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
[Route("[controller]/[action]")]
public sealed class NotebooksController : ControllerBase
{
    private readonly INotebooks _notebooks;

    public NotebooksController(INotebooks notebooks)
    {
        _notebooks = notebooks;
    }

    [HttpGet("")]
    public async Task<Page<Models.Note>> All(
        int index,
        int? count,
        int? offset,
        CancellationToken token)
    {
        var pagination = count is not null || offset is not null
            ? new Pagination(
                    Offset: offset.ToMaybe(),
                    Count: count.ToMaybe())
                ._(Maybe.Some)
            : Maybe.None<Pagination>();

        var page = await _notebooks.All(index, pagination, token);

        return new Page<Models.Note>(
            Items: page.Items
                .Select(_ => new Models.Note(
                    _.Id,
                    _.Value.Text,
                    _.Value.Pluses.ToByte()))
                .ToArray(),
            CurrentIndex: page.CurrentIndex,
            TotalItems: page.TotalItems);
    }


    [HttpPost("")]
    public Task<AddResult> StartAdd(
        int index,
        IReadOnlyCollection<string> values,
        int? origin,
        Direction direction,
        bool exact,
        CancellationToken token)
    {
        var added = origin is { } o
            ? new Added(values, exact, Maybe.Some(o), direction)
            : new Added(values);

        return _notebooks
            .Add(index, added, token)
            .ToModel();
    }

    [HttpPost("")]
    public Task<AddResult> StartActualize(
        int notebookId,
        int valueId,
        CancellationToken token)
        => _notebooks
            .Actualize(notebookId, valueId, token)
            .ToModel();

    [HttpPost("")]
    public Task<AddResult> ContinueAdd(
        int index,
        Guid id,
        bool isRight,
        CancellationToken token)
        => _notebooks.Add(index, id, isRight, token).ToModel();

    [HttpGet("")]
    public async Task<Models.Maybe<Models.Note>> Read(int index, int id, CancellationToken token)
    {
        var note = await _notebooks.Read(index, id, token);

        return note.Match(
            _ => new Models.Maybe<Models.Note>(true, new Models.Note(id, _.Text, _.Pluses.ToByte())),
            () => new Models.Maybe<Models.Note>(false, new Models.Note(default, string.Empty, default)));
    }

    [HttpPost("")]
    public Task Update(int index, int id, string name, CancellationToken token)
        => _notebooks.Update(index, id, name, token);

    [HttpPost("")]
    public Task Delete(int index, int id, CancellationToken token)
        => _notebooks.Delete(index, id, token);

    [HttpPost("")]
    public Task<Models.Either<byte, int>> Increment(int index, int id, CancellationToken token)
        => _notebooks.Increment(index, id, token).ToModel();

    [HttpPost("")]
    public Task<Models.Either<byte, int>> Decrement(int index, int id, CancellationToken token)
        => _notebooks.Decrement(index, id, token).ToModel();
}