using System;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.App.Client;
using TorteLand.Bot.Logic;

namespace TorteLand.Bot.StateMachine;

internal sealed class NotebooksState : BaseState
{
    private readonly INotebooksAcrudClient _client;
    private readonly int _pageSize;
    private int _offset;

    public NotebooksState(int pageSize, INotebooksAcrudClient client, IStateMachine context)
        : base(context)
    {
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
            "create" => Create(command.GetLine(), token),
            "open" => Open(command.GetInt(), token),
            "rename" => Rename(command.GetInt(), command.GetLine(1), token),
            "delete" or "remove" => Delete(command.GetInt(), token),
            _ => throw new Exception($"Unknown command: {command.Name}")
        };

    public override Task<string> Process(CancellationToken token) => All(_offset, token);

    private async Task<string> Rename(int id, string text, CancellationToken token)
    {
        await _client.RenameAsync(id, text, token);
        return await All(_offset, token);
    }

    private Task<string> Open(int index, CancellationToken token)
        => Context.ToNotebookState(index, token);

    private async Task<string> Delete(int index, CancellationToken token)
    {
        await _client.DeleteAsync(index, token);
        return await All(_offset, token);
    }

    private async Task<string> Create(string name, CancellationToken token)
    {
        var id = await _client.CreateAsync(name, token);
        return $"created: {id}";
    }

    private async Task<string> All(int offset, CancellationToken token)
    {
        offset = Math.Max(0, offset);

        var page = await _client.AllAsync(_pageSize, offset * _pageSize, token);

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

        foreach (var line in page.Items.Select(_ => $"{_.Id}. {_.Value}"))
            result.AppendLine(line);

        return result.ToString();
    }
}