using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.App.Client;

namespace TorteLand.Bot.Bot;

internal sealed class Bot : IBot
{
    private const string Undefined = "~";

    private readonly INotebooksAcrudClient _client;

    public Bot(IClientFactory factory)
    {
        _client = factory.CreateNotebooksAcrudClient();
    }

    public async Task<Maybe<string>> Process(string input, CancellationToken token)
    {
        var converted = input.Trim().Split(' ');
        if (converted is not { Length: >= 1 and <= 2 })
            throw new Exception($"Wrong command format: '{input}'");

        var command = converted[0].ToLowerInvariant();
        var arg = converted.Length > 1 ? converted[1] : Undefined;

        var response = await ProcessInternal(command, arg, token);

        return (response is { Length: > 0 } ? response : "{}")
            ._(Maybe.Some);
    }

    private Task<string> ProcessInternal(string input, string argument, CancellationToken token)
        => input switch
        {
            "status" => Task.FromResult("running"),
            "all" => All(token),
            "create" => Create(argument, token),
            "delete" => Delete(argument, token),
            _ => Task.FromResult("unknown command")
        };

    private async Task<string> Delete(string argument, CancellationToken token)
    {
        if (argument == Undefined)
            throw new Exception("Missing argument");

        var index = int.Parse(argument);
        await _client.DeleteAsync(index, token);

        return "deleted";
    }

    private async Task<string> Create(string argument, CancellationToken token)
    {
        if (argument == Undefined)
            throw new Exception("Missing argument");

        var id = await _client.CreateAsync(argument, token);
        return $"created: {id}";
    }

    private async Task<string> All(CancellationToken token)
    {
        var all = await _client.AllAsync(token);

        return all
               .Select(_ => $"{_.Id}. {_.Value}")
               ._(_ => string.Join(Environment.NewLine, _));
    }
}