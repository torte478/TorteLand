using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Logic;
using TorteLand.Bot.Utils;

namespace TorteLand.Bot.StateMachine;

internal sealed class StateMachine : IStateMachine
{
    private readonly IClientFactory _factory;
    private readonly IRandom _random;

    private IState _state = null!;

    public StateMachine(IClientFactory factory, IRandom random)
    {
        _factory = factory;
        _random = random;
    }

    public void SetState(IState state)
    {
        _state = state;
    }

    public Task<string> Process(ICommand command, CancellationToken token)
        => _state.Process(command, token);

    public Task<string> ToNotebooksState(CancellationToken token)
    {
        var next = new NotebooksState(_factory.CreateNotebooksAcrudClient(), this);
        return SetState(next, token);
    }

    public Task<string> ToNotebookState(int index, CancellationToken token)
    {
        var next = new NotebookState(index, _factory.CreateNotebooksClient(), this);
        return SetState(next, token);
    }

    public Task<string> ToNotebookAddState(
        int key,
        IReadOnlyCollection<string> notes,
        Guid transaction,
        string note,
        CancellationToken token)
    {
        var next = new NotebookAddState(key, notes, transaction, note, _factory.CreateNotebooksClient(), _random, this);
        return SetState(next, token);
    }

    private async Task<string> SetState(IState state, CancellationToken token)
    {
        var result = await state.Process(token);
        _state = state;
        return result;
    }
}