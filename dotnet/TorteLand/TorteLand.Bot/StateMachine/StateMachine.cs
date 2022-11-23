using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Integration;
using TorteLand.Bot.StateMachine.States;
using TorteLand.Bot.Utils;

namespace TorteLand.Bot.StateMachine;

internal sealed class StateMachine : IStateMachine
{
    private readonly IClientFactory _factory;
    private readonly IRandom _random;
    private readonly int _pageSize;

    private IState _state = null!;

    public StateMachine(int pageSize, IClientFactory factory, IRandom random)
    {
        _factory = factory;
        _random = random;
        _pageSize = pageSize;
    }

    public void SetState(IState state)
    {
        _state = state;
    }

    public Task<string> Process(ICommand command, CancellationToken token)
        => _state.Process(command, token);

    public Task<string> ToNotebooksState(CancellationToken token)
    {
        var next = new NotebooksState(_pageSize, _factory.CreateNotebooksAcrudClient(), this);
        return SetState(next, token);
    }

    public Task<string> ToNotebookState(int index, CancellationToken token)
    {
        var next = new NotebookState(index, _pageSize, _factory.CreateNotebooksClient(), this);
        return SetState(next, token);
    }

    public Task<string> ToNotebookAddState(
        int key,
        IReadOnlyCollection<string> notes,
        Guid transaction,
        string note,
        CancellationToken token)
    {
        var context = new NotebookAddStateContext(
            key,
            notes,
            transaction,
            _factory.CreateNotebooksClient(),
            _random);
        
        var next = new NotebookAddState(context, note, this);
        return SetState(next, token);
    }

    public Task<string> ToConfirmActionState(
        string question,
        Func<CancellationToken, Task<string>> onAction,
        CancellationToken token)
    {
        var next = new ConfirmActionState(question, _state, onAction, this);
        return SetState(next, token);
    }

    public override string ToString()
        => $"{GetType().Name}[{_state.GetType().Name}]";

    private async Task<string> SetState(IState state, CancellationToken token)
    {
        var result = await state.Process(token);
        _state = state;
        return result;
    }
}