using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.App.Client;
using TorteLand.Bot.Logic;

namespace TorteLand.Bot.StateMachine;

internal sealed class NotebooksState : BaseState
{
    private readonly INotebooksAcrudClient _client;

    public NotebooksState(IStateMachine context, IClientFactory factory)
        : base(context, factory)
    {
        _client = factory.CreateNotebooksAcrudClient();
    }

    public override Task<string> Process(ICommand command, CancellationToken token)
        => command.Name switch
        {
            "all" => All(token),
            "create" => Create(command.GetStringArgument(), token),
            "delete" => Delete(command.GetIntArgument(), token),
            "open" => Open(command.GetIntArgument(), token),
            _ => throw new Exception($"Unknown command: {command.Name}")
        };

    public override Task<string> Process(CancellationToken token) => All(token);

    private Task<string> Open(int index, CancellationToken token)
    {
        var next = new NotebookState(index, Context, Factory);
        return Context.SetState(next, token);
    }

    private async Task<string> Delete(int index, CancellationToken token)
    {
        await _client.DeleteAsync(index, token);
        return await All(token);
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