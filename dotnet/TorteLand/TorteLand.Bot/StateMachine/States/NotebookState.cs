using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.App.Client;
using TorteLand.Bot.Integration;
using TorteLand.Extensions;

namespace TorteLand.Bot.StateMachine.States;

internal sealed class NotebookState : BaseState
{
    private readonly int _key;
    private readonly INotebooksClient _client;
    private readonly int _pageSize;

    private int _offset;

    public NotebookState(int key, int pageSize, INotebooksClient client, IStateMachine machine)
        : base(machine)
    {
        _key = key;
        _client = client;
        _pageSize = pageSize;
        _offset = 0;
    }

    public override Task<string> Process(ICommand command, CancellationToken token)
    {
        var (name, arguments) = command.ToName();
        return name switch
        {
            "all" => All(_offset, token),
            "next" => All(_offset + 1, token),
            "back" => All(_offset - 1, token),
            "add" or "доб" => StartAdd(arguments, token),
            "rename" => Rename(arguments, token),
            "delete" or "remove" => Delete(arguments, token),
            "close" => Close(token),
            
            _ => name.ToUnknown()
        };
    }

    public override Task<string> Process(CancellationToken token) => All(_offset, token);

    private Task<string> Close(CancellationToken token)
        => Machine.ToNotebooksState(token);

    private async Task<string> All(int offset, CancellationToken token)
    {
        offset = Math.Max(offset, 0);

        var page = await _client.AllAsync(_key, _pageSize, offset * _pageSize,  token);

        if (page.Items.Count > 0)
            _offset = offset;

        var result = new StringBuilder();

        if (page.Items.Count < page.TotalItems && page.Items.Count > 0)
            string.Format(
                      "{0}..{1} from {2}",
                      _offset * _pageSize,
                      _offset * _pageSize + page.Items.Count,
                      page.TotalItems)
                  ._(result.AppendLine)
                  .AppendLine();

        foreach (var line in page.Items.Select(_ => $"{_.Key}. {_.Value}"))
            result.AppendLine(line);

        return result.ToString();
    }

    private async Task<string> StartAdd(ICommand command, CancellationToken token)
    {
        var notes = command.ToLines();

        var response = await _client.StartAddAsync(_key, notes, token);
        if (response.Right is null)
            return await All(_offset, token);

        var guid = response.Right.Id;
        var item = response.Right.Text;
        return await Machine.ToNotebookAddState(_key, notes, guid, item, token);
    }

    private async Task<string> Delete(ICommand command, CancellationToken token)
    {
        var (index, _) = command.ToInt();
        var name = await _client.ReadAsync(_key, index, token);

        if (!name.IsSome)
            return $"Wrong index: {index}";

        return await Machine.ToConfirmActionState(
                   $"Delete '{name.Value}'?",
                   ct => Delete(index, ct),
                   token);
    }
    
    private async Task<string> Delete(int index, CancellationToken token)
    {
        await _client.DeleteAsync(_key , index, token);
        return await Process(token);
    }

    private async Task<string> Rename(ICommand command, CancellationToken token)
    {
        var (id, tail) = command.ToInt();
        var (name, _) = tail.ToLine();

        await _client.UpdateAsync(_key, id, name, token);
        return await All(_offset, token);
    }
}