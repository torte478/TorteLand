using System;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Integration;

namespace TorteLand.Bot.StateMachine;

internal sealed class ConfirmActionState : BaseState
{
    private readonly string _question;
    private readonly Func<CancellationToken, Task<string>> _onAction;
    private readonly Func<CancellationToken, Task<string>> _onCancel;

    public ConfirmActionState(
        string question,
        Func<CancellationToken, Task<string>> onAction,
        Func<CancellationToken, Task<string>> onCancel,
        IStateMachine context)
        : base(context)
    {
        _question = question;
        _onAction = onAction;
        _onCancel = onCancel;
    }

    public override Task<string> Process(ICommand command, CancellationToken token)
    {
        var (input, _) = command.ToWord();

        return input switch
        {
            "y" => _onAction(token),
            _ => _onCancel(token)
        };
    }

    public override Task<string> Process(CancellationToken token)
        => _question._(Task.FromResult);
}