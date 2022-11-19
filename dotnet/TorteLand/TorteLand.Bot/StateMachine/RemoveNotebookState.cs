using System.Threading;
using System.Threading.Tasks;
using TorteLand.App.Client;
using TorteLand.Bot.Logic;

namespace TorteLand.Bot.StateMachine;

internal sealed class RemoveNotebookState : BaseState
{
    private readonly INotebooksAcrudClient _client;
    private readonly int _index;
    private readonly string _name;

    public RemoveNotebookState(int index, string name, INotebooksAcrudClient client, IStateMachine context)
        : base(context)
    {
        _index = index;
        _name = name;
        _client = client;
    }

    public override Task<string> Process(ICommand command, CancellationToken token)
    {
        var (input, _) = command.ToWord();

        return input switch
        {
            "y" => Delete(token),
            _ => GoBack(token)
        };
    }

    private Task<string> GoBack(CancellationToken token)
        => Context.ToNotebooksState(token);

    public override Task<string> Process(CancellationToken token)
        => $"Delete '{_name}' ?"._(Task.FromResult);

    private async Task<string> Delete(CancellationToken token)
    {
        await _client.DeleteAsync(_index, token);
        return await Context.ToNotebooksState(token);
    }
}