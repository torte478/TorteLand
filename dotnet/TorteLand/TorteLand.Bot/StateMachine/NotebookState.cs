using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.App.Client;
using TorteLand.Bot.Logic;

namespace TorteLand.Bot.StateMachine;

internal sealed class NotebookState : BaseState
{
    private readonly int _key;
    private readonly INotebooksClient _client;

    public NotebookState(int key, IStateMachine context, IClientFactory factory)
        : base(context, factory)
    {
        _key = key;
        _client = factory.CreateNotebooksClient();
    }

    public override Task<string> Process(ICommand command, CancellationToken token)
        => command.Name switch
        {
            "all" => All(token),
            "add" => StartAdd(command.GetStringArgument(), token),
            "delete" => Delete(command.GetIntArgument(), token),
            "close" => Close(token),
            _ => throw new Exception($"Unknown command: {command.Name}")
        };

    public override Task<string> Process(CancellationToken token) => All(token);

    private Task<string> Close(CancellationToken token)
    {
        var next = new NotebooksState(Context, Factory);
        return Context.SetState(next, token);
    }

    private async Task<string> All(CancellationToken token)
    {
        var all = await _client.AllAsync(_key, token);
        return all
               .Select(_ => $"{_.Key}. {_.Value}")
               ._(_ => string.Join(Environment.NewLine, _)); // TODO: to string builder
    }

    private async Task<string> StartAdd(string note, CancellationToken token)
    {
        var response = await _client.StartAddAsync(_key, note, token);
        if (response.Right is null)
            return await All(token);

        var guid = response.Right.Id!.Value;
        var item = response.Right.Text;
        var next = new NotebookAddState(_key, note, guid, item, Context, Factory);
        return await Context.SetState(next, token);
    }

    private async Task<string> Delete(int index, CancellationToken token)
    {
        await _client.DeleteAsync(_key, index, token);
        return await All(token);
    }
}