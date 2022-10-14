using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Bot;

namespace TorteLand.Bot.StateMachine;

internal interface IStateMachine
{
    Task<string> SetState(IState state, CancellationToken token);
    Task<string> Process(ICommand command, CancellationToken token);
    void SetState(IState state);
}