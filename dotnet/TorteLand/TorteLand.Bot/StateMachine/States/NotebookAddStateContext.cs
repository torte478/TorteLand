using System;
using System.Collections.Generic;
using System.Linq;
using TorteLand.App.Client;
using TorteLand.Bot.Utils;

namespace TorteLand.Bot.StateMachine.States;

internal sealed record NotebookAddStateContext(
    int Key,
    IReadOnlyCollection<string> Notes,
    Guid Transaction,
    INotebooksClient Client,
    IRandom Random)
{
    public string Origin => Notes.Count > 1
                                ? $"[..{Notes.First()}]"
                                : Notes.First();
}