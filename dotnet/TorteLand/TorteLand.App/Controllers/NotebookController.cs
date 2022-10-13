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
    public async Task<Models.Either<int, Segment>> Add(
        int index,
        string value,
        CancellationToken token)
    {
        var result = await _notebooks.Add(
                         index,
                         value,
                         Maybe.None<ResolvedSegment>(),
                         token);

        return result.Match(
            x => new Models.Either<int, Segment>(x, default),
            x => new Models.Either<int, Segment>(default, x));
    }

    [HttpPost]
    [Route("continue_add")]
    public async Task<Models.Either<int, Segment>> Add2(
        int index,
        string value,
        ResolvedSegment segment,
        CancellationToken token)
    {
        var result = await _notebooks.Add(index, value, Maybe.Some(segment), token);

        return result.Match(
            x => new Models.Either<int, Segment>(x, default),
            x => new Models.Either<int, Segment>(default, x));
    }
}