using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.App.Client;

namespace TorteLand.Bot;

internal sealed class Bot : IBot
{
    private readonly INotebooksAcrudClient _client;

    public Bot(IClientFactory factory)
    {
        _client = factory.CreateNotebooksAcrudClient();
    }

    public async Task<Maybe<string>> Process(string input, CancellationToken token)
    {
        var converted = input.ToLowerInvariant().Trim();

        var response = await ProcessInternal(converted, token);

        return (response is { Length: > 0 } ? response : "{}")
            ._(Maybe.Some);

    }

    private async Task<string> ProcessInternal(string input, CancellationToken token)
    {
        if (input == "all")
        {
            var all = await _client.AllAsync(token);
            return all
                   .Select(_ => $"{_.Id}. {_.Value}")
                   ._(_ => string.Join(Environment.NewLine, _));
        }

        return "unknown command";
    }
}