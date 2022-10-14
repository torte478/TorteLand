using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Bot;

namespace TorteLand.Bot.StateMachine;

internal interface IState
{
    Task<string> Process(ICommand command, CancellationToken token);
    Task<string> Process(CancellationToken token);
}