using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftwareCraft.Functional;
using TorteLand.Core;

namespace TorteLand.App.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class NotebookController : ControllerBase
{
    private readonly INotebookProvider<int> _provider;

    public NotebookController(INotebookProvider<int> provider)
    {
        _provider = provider;
    }

    [HttpGet]
    public async IAsyncEnumerable<KeyValuePair<int, string>> All(
        int index,
        [EnumeratorCancellation] CancellationToken token)
    {
        var notebook = _provider.Get<int, string>(index);

        await foreach (var item in notebook.WithCancellation(token))
        {
            yield return new KeyValuePair<int, string>(item.Item1, item.Item2);
        }
    }

    [HttpPost]
    public async Task<Models.Either<int, Segment<int>>> Add(
        int index,
        string value,
        HalfSegment<int>? segment,
        CancellationToken token)
    {
        var notebook = _provider.Get<int, string>(index);
        var maybe = segment is { }
                        ? Maybe.Some(segment)
                        : Maybe.None<HalfSegment<int>>();

        var result = await notebook.Add(value, maybe, token);
        return result.Match(
            x => new Models.Either<int, Segment<int>>(x, default),
            x => new Models.Either<int, Segment<int>>(default, x));
    }
}