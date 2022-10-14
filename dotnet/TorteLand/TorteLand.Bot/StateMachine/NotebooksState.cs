using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.App.Client;
using TorteLand.Bot.Bot;

namespace TorteLand.Bot.StateMachine;

internal sealed class NotebooksState : IState
{
    private readonly IStateMachine _context;
    private readonly IClientFactory _factory;
    private readonly INotebooksAcrudClient _client;

    public NotebooksState(IStateMachine context, IClientFactory factory)
    {
        _context = context;
        _factory = factory;

        _client = factory.CreateNotebooksAcrudClient();
    }

    public Task<string> Process(ICommand command, CancellationToken token)
        => command.Name switch
        {
            "all" => All(token),
            "create" => Create(command.GetStringArgument(), token),
            "delete" => Delete(command.GetIntArgument(), token),
            "open" => Open(command.GetIntArgument(), token),
            _ => throw new Exception($"Unknown command: {command.Name}")
        };

    public Task<string> Process(CancellationToken token) => All(token);

    private Task<string> Open(int index, CancellationToken token)
    {
        var next = new NotebookState(index, _context, _factory);
        return _context.SetState(next, token);
    }

    private async Task<string> Delete(int index, CancellationToken token)
    {
        await _client.DeleteAsync(index, token);

        return "deleted";
    }

    private async Task<string> Create(string name, CancellationToken token)
    {
        var id = await _client.CreateAsync(name, token);
        return $"created: {id}";
    }

    private async Task<string> All(CancellationToken token)
    {
        var all = await _client.AllAsync(token);

        return all
               .Select(_ => $"{_.Id}. {_.Value}")
               ._(_ => string.Join(Environment.NewLine, _));// TODO: to string builder
    }
}