using System;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Integration;
using TorteLand.Extensions;

namespace TorteLand.Bot.StateMachine.States;

internal sealed class NotebookAddState : BaseState
{
    private readonly NotebookAddStateContext _context;

    private string _note;

    public NotebookAddState(
        NotebookAddStateContext context,
        string note,
        IStateMachine machine)
        : base(machine)
    {
        _context = context;
        _note = note;
    }

    public override Task<string> Process(ICommand command, CancellationToken token)
    {
        var (name, _) = command.ToName();
        return name switch
        {
            "y" or "д" => Add(true, token),
            "n" or "н" => Add(false, token),
            "?" => Add(_context.Random.Next(2) == 0, token),
            "cancel" or "отмена" => GoBack(token),
            _ => throw new Exception($"Unknown command: {name}")
        };
    }

    public override Task<string> Process(CancellationToken token)
        => GetQuestion()._(Task.FromResult);

    private async Task<string> Add(bool isRight, CancellationToken token)
    {
        var response = await _context.Client.ContinueAddAsync(
                           _context.Key,
                           _context.Transaction,
                           isRight,
                           token);

        if (response.Right is null)
            return await GoBack(token);

        _note = response.Right.Text;
        return GetQuestion();
    }

    private Task<string> GoBack(CancellationToken token)
        => Machine.ToNotebookState(_context.Key, token);

    private string GetQuestion()
        => string.Format("{0}{1}is greater than{1}{1}{2}", _context.Origin, Environment.NewLine, _note);
}