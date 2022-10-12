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
    private readonly INotebooks<int, int, string> _notebooks;

    public NotebookController(INotebooks<int, int, string> notebooks)
    {
        _notebooks = notebooks;
    }

    [HttpGet]
    public IAsyncEnumerable<KeyValuePair<int, string>> All(
        int index,
        CancellationToken token)
        => _notebooks
           .All(index, token)
           .Select(_ => new KeyValuePair<int, string>(_.Item1, _.Item2));

    [HttpPut]
    public int Create() => _notebooks.Create();

    [HttpPost]
    [Route("start_add")]
    public async Task<Models.Either<int, Segment<int>>> Add(
        int index,
        string value,
        CancellationToken token)
    {
        var result = await _notebooks.Add(
                         index,
                         value,
                         Maybe.None<HalfSegment<int>>(),
                         token);

        return result.Match(
            x => new Models.Either<int, Segment<int>>(x, default),
            x => new Models.Either<int, Segment<int>>(default, x));
    }

    [HttpPost]
    [Route("continue_add")]
    public async Task<Models.Either<int, Segment<int>>> Add2(
        int index,
        string value,
        HalfSegment<int> segment,
        CancellationToken token)
    {
        var result = await _notebooks.Add(index, value, Maybe.Some(segment), token);

        return result.Match(
            x => new Models.Either<int, Segment<int>>(x, default),
            x => new Models.Either<int, Segment<int>>(default, x));
    }
}