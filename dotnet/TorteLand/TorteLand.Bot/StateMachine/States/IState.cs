using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Integration;

namespace TorteLand.Bot.StateMachine.States;

internal interface IState
{
    Task<string> Process(ICommand command, CancellationToken token);
    Task<string> Process(CancellationToken token);
}