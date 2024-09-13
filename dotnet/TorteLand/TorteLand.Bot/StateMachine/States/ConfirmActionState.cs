using System;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Integration;
using TorteLand.Extensions;

namespace TorteLand.Bot.StateMachine.States;

internal sealed class ConfirmActionState : BaseState
{
    private readonly string _question;
    private readonly IState _previous;
    private readonly Func<CancellationToken, Task> _onAction;

    public ConfirmActionState(
        string question,
        IState previous,
        Func<CancellationToken, Task> onAction,
        IStateMachine machine)
        : base(machine)
    {
        _question = question;
        _previous = previous;
        _onAction = onAction;
    }

    public override async Task<string> Process(ICommand command, CancellationToken token)
    {
        var (input, _) = command.ToWord();

        if (input.Trim().ToLowerInvariant()== "y")
            await _onAction(token);
        
        Machine.SetState(_previous);
        return await _previous.Process(token);
    }

    public override Task<string> Process(CancellationToken token)
        => _question._(Task.FromResult);
}