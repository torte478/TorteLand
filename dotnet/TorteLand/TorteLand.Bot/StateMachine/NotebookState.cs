using System;
using System.Collections.Generic;
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

    public NotebookState(int key, INotebooksClient client, IStateMachine context)
        : base(context)
    {
        _key = key;
        _client = client;
    }

    public override Task<string> Process(ICommand command, CancellationToken token)
        => command.Name switch
        {
            "all" => All(token),
            "add" or "доб" => StartAdd(command.GetLines(), token),
            "rename" => Rename(command.GetInt(), command.GetLine(1), token),
            "delete" => Delete(command.GetInt(), token),
            "close" => Close(token),
            _ => throw new Exception($"Unknown command: {command.Name}")
        };

    public override Task<string> Process(CancellationToken token) => All(token);

    private Task<string> Close(CancellationToken token)
        => Context.ToNotebooksState(token);

    private async Task<string> All(CancellationToken token)
    {
        var all = await _client.AllAsync(_key, token);
        return all
               .Select(_ => $"{_.Key}. {_.Value}")
               ._(_ => string.Join(Environment.NewLine, _)); // TODO: to string builder
    }

    private async Task<string> StartAdd(IReadOnlyCollection<string> notes, CancellationToken token)
    {
        var response = await _client.StartAddAsync(_key, notes, token);
        if (response.Right is null)
            return await All(token);

        var guid = response.Right.Id;
        var item = response.Right.Text;
        return await Context.ToNotebookAddState(_key, notes, guid, item, token);
    }

    private async Task<string> Delete(int index, CancellationToken token)
    {
        await _client.DeleteAsync(_key, index, token);
        return await All(token);
    }

    private async Task<string> Rename(int id, string text, CancellationToken token)
    {
        await _client.RenameAsync(_key, id, text, token);
        return await All(token);
    }
}