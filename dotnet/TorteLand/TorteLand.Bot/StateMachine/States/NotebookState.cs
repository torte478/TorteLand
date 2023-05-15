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
            "plus" or "+" => Increment(arguments, token),
            "minus" or "-" => Decrement(arguments, token),
            
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

        foreach (var line in page.Items.Select(ToNoteString))
            result.AppendLine(line);

        return result.ToString();
    }

    private static string ToNoteString(Note note)
    {
        if (note.Pluses == 0)
            return $"{note.Id}. {note.Text}";

        var pluses = Enumerable
                     .Range(0, note.Pluses)
                     .Select(_ => '+')
                     ._(_ => string.Join(string.Empty, _));
        return $"{note.Id}. {note.Text} {pluses}";
    }

    private async Task<string> StartAdd(ICommand command, CancellationToken token)
    {
        var notes = command.ToLines();
        var (exact, direction, origin, note) = ParseAddedOptions(notes.First());
        var body = note.AsArray().Concat(notes.Skip(1));

        Console.WriteLine(
            $"{exact} {direction} {origin} {note}");

        var response = await _client.StartAddAsync(
                           index: _key,
                           origin: origin,
                           direction: direction,
                           exact: exact,
                           body: body,
                           cancellationToken: token);
                           
        if (response.Right is null)
            return await All(_offset, token);

        var guid = response.Right.Id;
        var item = response.Right.Text;
        return await Machine.ToNotebookAddState(_key, notes, guid, item, token);
    }

    private (bool Exact, Direction Direction, int? Origin, string? Note) ParseAddedOptions(string line)
    {
        var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 1 || tokens[0] is not ("exact" or "after" or "before"))
            return (false, default, null, line);

        var exact = tokens[0] == "exact";
        var directionIndex = exact ? 1 : 0;
        var direction = tokens[directionIndex] == "before" ? Direction._0 : Direction._1;
        var origin = tokens[directionIndex + 1]._(int.Parse);
        var note = tokens.Skip(directionIndex + 2)._(_ => string.Join(' ', _));

        return (exact, direction, origin, note);
    }

    private async Task<string> Delete(ICommand command, CancellationToken token)
    {
        var (index, _) = command.ToInt();
        var name = await _client.ReadAsync(_key, index, token);

        if (!name.IsSome)
            return $"Wrong index: {index}";

        return await Machine.ToConfirmActionState(
                   $"Delete '{name.Value.Text}'?",
                   ct => Delete(index, ct),
                   token);
    }
    
    private Task<string> Increment(ICommand command, CancellationToken token)
        => ChangePlusCount(command, _client.IncrementAsync, token);
    
    private Task<string> Decrement(ICommand command, CancellationToken token)
        => ChangePlusCount(command, _client.DecrementAsync, token);
    
    private async Task<string> ChangePlusCount(
        ICommand command, 
        Func<int?, int?, CancellationToken, Task<ByteInt32Either>> f, 
        CancellationToken token)
    {
        var (index, _) = command.ToInt();
        var name = await _client.ReadAsync(_key, index, token);

        if (!name.IsSome)
            return $"Wrong index: {index}";

        await f(_key, index, token);
        return await All(_offset, token);
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