using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.Bot.Integration;
using TorteLand.Bot.StateMachine.States;

namespace TorteLand.Bot.StateMachine;

internal interface IStateMachine
{
    void SetState(IState state);

    Task<string> Process(ICommand command, CancellationToken token);

    Task<string> ToNotebooksState(CancellationToken token);
    Task<string> ToNotebookState(int index, CancellationToken token);
    Task<string> ToNotebookAddState(
        int key,
        IReadOnlyCollection<string> notes,
        Guid transaction,
        string note,
        CancellationToken token);

    Task<string> ToConfirmActionState(
        string question,
        Func<CancellationToken, Task<string>> onAction,
        CancellationToken token);
}