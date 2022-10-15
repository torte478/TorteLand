using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.App.Client;
using TorteLand.Bot.Logic;
using TorteLand.Bot.Utils;

namespace TorteLand.Bot.StateMachine;

internal sealed class NotebookAddState : BaseState
{
    private readonly int _key;
    private readonly Guid _transaction;
    private readonly string _origin;
    private readonly INotebooksClient _client;
    private readonly IRandom _random;

    private string _note;

    // TODO : arguments
    public NotebookAddState(
        int key,
        IReadOnlyCollection<string> notes,
        Guid transaction,
        string note,
        INotebooksClient client,
        IRandom random,
        IStateMachine context)
        : base(context)
    {
        _key = key;
        _transaction = transaction;
        _note = note;
        _client = client;
        _random = random;

        _origin = notes.Count > 1
                      ? $"[..{notes.First()}]"
                      : notes.First();
    }

    public override Task<string> Process(ICommand command, CancellationToken token)
    {
        var (name, _) = command.ToName();
        return name switch
        {
            "y" or "д" => Add(true, token),
            "n" or "н" => Add(false, token),
            "?" => Add(_random.Next(2) == 0, token),
            "cancel" or "отмена" => GoBack(token),
            _ => throw new Exception($"Unknown command: {name}")
        };
    }

    public override Task<string> Process(CancellationToken token)
        => GetQuestion()._(Task.FromResult);

    private async Task<string> Add(bool isRight, CancellationToken token)
    {
        var response = await _client.ContinueAddAsync(_key, _transaction, isRight, token);

        if (response.Right is null)
            return await GoBack(token);

        _note = response.Right.Text;
        return GetQuestion();
    }

    private Task<string> GoBack(CancellationToken token)
        => Context.ToNotebookState(_key, token);

    private string GetQuestion()
        => string.Format("{0}{1}is greater than{1}{1}{2}", _origin, Environment.NewLine, _note);
}