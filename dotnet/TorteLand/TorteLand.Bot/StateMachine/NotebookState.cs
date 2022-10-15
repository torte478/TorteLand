using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.App.Client;
using TorteLand.Bot.Logic;

namespace TorteLand.Bot.StateMachine;

internal sealed class NotebookState : BaseState
{
    private readonly int _key;
    private readonly INotebooksClient _client;
    private readonly int _pageSize;

    private int _offset;

    public NotebookState(int key, int pageSize, INotebooksClient client, IStateMachine context)
        : base(context)
    {
        _key = key;
        _client = client;
        _pageSize = pageSize;
        _offset = 0;
    }

    public override Task<string> Process(ICommand command, CancellationToken token)
        => command.Name switch
        {
            "all" => All(_offset, token),
            "next" => All(_offset + 1, token),
            "back" => All(_offset - 1, token),
            "add" or "доб" => StartAdd(command.GetLines(), token),
            "rename" => Rename(command.GetInt(), command.GetLine(1), token),
            "delete" or "remove" => Delete(command.GetInt(), token),
            "close" => Close(token),
            _ => throw new Exception($"Unknown command: {command.Name}")
        };

    public override Task<string> Process(CancellationToken token) => All(_offset, token);

    private Task<string> Close(CancellationToken token)
        => Context.ToNotebooksState(token);

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

    private async Task<string> StartAdd(IReadOnlyCollection<string> notes, CancellationToken token)
    {
        var response = await _client.StartAddAsync(_key, notes, token);
        if (response.Right is null)
            return await All(_offset, token);

        var guid = response.Right.Id;
        var item = response.Right.Text;
        return await Context.ToNotebookAddState(_key, notes, guid, item, token);
    }

    private async Task<string> Delete(int index, CancellationToken token)
    {
        await _client.DeleteAsync(_key, index, token);
        return await All(_offset, token);
    }

    private async Task<string> Rename(int id, string text, CancellationToken token)
    {
        await _client.RenameAsync(_key, id, text, token);
        return await All(_offset, token);
    }
}