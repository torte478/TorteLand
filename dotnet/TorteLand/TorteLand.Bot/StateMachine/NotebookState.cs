using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.App.Client;
using TorteLand.Bot.Bot;

namespace TorteLand.Bot.StateMachine;

internal sealed class NotebookState : IState
{
    private readonly int _index;
    private readonly IStateMachine _context;
    private readonly IClientFactory _factory;
    private readonly INotebooksClient _client;

    public NotebookState(int index, IStateMachine context, IClientFactory factory)
    {
        _context = context;
        _factory = factory;
        _index = index;
        _client = factory.CreateNotebooksClient();
    }

    public Task<string> Process(ICommand command, CancellationToken token)
        => command.Name switch
        {
            "all" => All(token),
            "close" => Close(token),
            _ => throw new Exception($"Unknown command: {command.Name}")
        };

    public Task<string> Process(CancellationToken token) => All(token);

    private Task<string> Close(CancellationToken token)
    {
        var next = new NotebooksState(_context, _factory);
        return _context.SetState(next, token);
    }

    private async Task<string> All(CancellationToken token)
    {
        var all = await _client.AllAsync(_index, token);
        return all
               .Select(_ => $"{_.Key}. {_.Value}")
               ._(_ => string.Join(Environment.NewLine, _)); // TODO: to string builder
    }
}