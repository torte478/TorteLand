using System;
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
    {
        var (name, arguments) = command.ToName();
        return name switch
        {
            "all" => All(_offset, token),
            "next" => All(_offset + 1, token),
            "back" => All(_offset - 1, token),

            "create" => Create(arguments, token),
            "open" => Open(arguments, token),
            "rename" => Rename(arguments, token),
            "delete" or "remove" => Delete(arguments, token),

            _ => throw new Exception($"Unknown command: {name}")
        };
    }

    public override Task<string> Process(CancellationToken token) => All(_offset, token);

    private async Task<string> Rename(ICommand command, CancellationToken token)
    {
        var (id, tail) = command.ToInt();
        var (text, _) = tail.ToLine();

        await _client.UpdateAsync(id, text, token);
        return await All(_offset, token);
    }

    private Task<string> Open(ICommand command, CancellationToken token)
    {
        var (index, _) = command.ToInt();
        return Context.ToNotebookState(index, token);
    }

    private async Task<string> Delete(ICommand command, CancellationToken token)
    {
        var (index, _) = command.ToInt();
        var name = await _client.ReadAsync(index, token);

        if (!name.IsSome)
            return $"Wrong index: {index}";

        return await Context.ToConfirmActionState(
                   $"Delete '{name.Value}'?",
                   ct => Delete(index, ct),
                   Context.ToNotebooksState,
                   token);
    }

    private async Task<string> Create(ICommand command, CancellationToken token)
    {
        var (name, _) = command.ToLine();

        var id = await _client.CreateAsync(name, token);
        return $"created: {id}";
    }

    private async Task<string> All(int offset, CancellationToken token)
    {
        var page = await Reload(offset, token);
        return All(page);
    }

    private async Task<StringUniquePage> Reload(int offset, CancellationToken token)
    {
        offset = Math.Max(0, offset);

        var page = await _client.AllAsync(_pageSize, offset * _pageSize, token);

        if (page.Items.Count > 0)
            _offset = offset;

        return page;
    }

    private string All(StringUniquePage page)
    {
        var result = new StringBuilder();

        if (page.Items.Count < page.TotalItems && page.Items.Count > 0)
            string.Format(
                      "{0}..{1} from {2}",
                      _offset * _pageSize,
                      _offset * _pageSize + page.Items.Count,
                      page.TotalItems)
                  ._(result.AppendLine)
                  .AppendLine();

        foreach (var line in page.Items)
            result.AppendLine($"{line.Id}. {line.Value}");

        return result.ToString();       
    }
    
    private async Task<string> Delete(int index, CancellationToken token)
    {
        await _client.DeleteAsync(index, token);
        return await Process(token);
    }
}