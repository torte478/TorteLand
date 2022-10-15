using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Logic;

namespace TorteLand.Bot.StateMachine;

internal abstract class BaseState : IState
{
    protected IStateMachine Context { get; }

    protected BaseState(IStateMachine context)
    {
        Context = context;
    }

    public abstract Task<string> Process(ICommand command, CancellationToken token);

    public abstract Task<string> Process(CancellationToken token);
}