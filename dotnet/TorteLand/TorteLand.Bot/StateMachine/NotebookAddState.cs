using System;
using System.Threading;
using System.Threading.Tasks;
using TorteLand.App.Client;
using TorteLand.Bot.Logic;

namespace TorteLand.Bot.StateMachine;

internal sealed class NotebookAddState : BaseState
{
    private readonly int _key;
    private readonly Guid _transaction;
    private readonly string _origin;
    private readonly INotebooksClient _client;

    private string _note;

    public NotebookAddState(int key, string origin, Guid transaction, string note, IStateMachine context, IClientFactory factory)
        : base(context, factory)
    {
        _key = key;
        _origin = origin;
        _transaction = transaction;
        _note = note;
        _client = factory.CreateNotebooksClient();
    }

    public override Task<string> Process(ICommand command, CancellationToken token)
    => command.Name switch
    {
        "y" => Add(true, token),
        "n" => Add(false, token),
        "cancel" => GoBack(token),
        _ => throw new Exception($"Unknown command: {command.Name}")
    };

    public override Task<string> Process(CancellationToken token)
        => GetQuestion()._(Task.FromResult);

    private async Task<string> Add(bool isRight, CancellationToken token)
    {
        var response = await _client.ContinueAddAsync(_key, _transaction, isRight, token);

        if (response.Right is null)
            return await GoBack(token);

        _note = response.Right.Text;
        return GetQuestion();
    }

    private Task<string> GoBack(CancellationToken token)
    {
        var next = new NotebookState(_key, Context, Factory);
        return Context.SetState(next, token);
    }

    private string GetQuestion()
        => string.Format("\"{0}\"{1}is greater than{1}\"{2}\"", _origin, Environment.NewLine, _note);
}