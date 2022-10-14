using TorteLand.Bot.Logic;
using TorteLand.Bot.Utils;

namespace TorteLand.Bot.StateMachine;

internal sealed class StateMachineFactory : IStateMachineFactory
{
    private readonly IClientFactory _factory;
    private readonly IRandom _random;

    public StateMachineFactory(IClientFactory factory, IRandom random)
    {
        _factory = factory;
        _random = random;
    }

    public IStateMachine Create()
    {
        var machine = new StateMachine(_factory, _random);
        var start = new NotebooksState(_factory.CreateNotebooksAcrudClient(), machine);
        machine.SetState(start);
        return machine;
    }
}