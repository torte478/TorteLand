using TorteLand.Bot.Logic;
using TorteLand.Bot.Utils;

namespace TorteLand.Bot.StateMachine;

internal sealed class StateMachineFactory : IStateMachineFactory
{
    private readonly int _pageSize;
    private readonly IClientFactory _factory;
    private readonly IRandom _random;

    public StateMachineFactory(int pageSize, IClientFactory factory, IRandom random)
    {
        _factory = factory;
        _random = random;
        _pageSize = pageSize;
    }

    public IStateMachine Create()
    {
        var machine = new StateMachine(_pageSize, _factory, _random);
        var start = new NotebooksState(_pageSize, _factory.CreateNotebooksAcrudClient(), machine);
        machine.SetState(start);
        return machine;
    }
}