using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Integration;

namespace TorteLand.Bot.StateMachine.States;

internal abstract class BaseState : IState
{
    protected IStateMachine Machine { get; }

    protected BaseState(IStateMachine machine)
    {
        Machine = machine;
    }

    public abstract Task<string> Process(ICommand command, CancellationToken token);

    public abstract Task<string> Process(CancellationToken token);
}