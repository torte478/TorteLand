using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Logic;

namespace TorteLand.Bot.StateMachine;

internal sealed class StateMachine : IStateMachine
{
    private IState _state = null!;

    public Task<string> Process(ICommand command, CancellationToken token)
        => _state.Process(command, token);

    public async Task<string> SetState(IState state, CancellationToken token)
    {
        var result = await state.Process(token);
        _state = state;
        return result;
    }

    public void SetState(IState state)
    {
        _state = state;
    }
}