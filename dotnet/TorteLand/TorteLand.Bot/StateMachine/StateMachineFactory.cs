using TorteLand.Bot.Bot;

namespace TorteLand.Bot.StateMachine;

internal sealed class StateMachineFactory : IStateMachineFactory
{
    private readonly IClientFactory _factory;

    public StateMachineFactory(IClientFactory factory)
    {
        _factory = factory;
    }

    public IStateMachine Create()
    {
        var machine = new StateMachine();
        var start = new NotebooksState(machine, _factory);
        machine.SetState(start);
        return machine;
    }
}