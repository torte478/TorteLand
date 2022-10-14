using System;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Bot.StateMachine;

namespace TorteLand.Bot.Bot;

internal sealed class Bot : IBot
{
    private readonly ICommandFactory _factory;
    private readonly IStateMachine _machine;

    public Bot(IStateMachineFactory stateMachineFactory, ICommandFactory factory)
    {
        _factory = factory;
        _machine = stateMachineFactory.Create();
    }

    public async Task<Maybe<string>> Process(string input, CancellationToken token)
    {
        var converted = input.Trim().Split(' ');
        if (converted is not { Length: >= 1 and <= 2 })
            throw new Exception($"Wrong command format: '{input}'");

        var command = _factory.Create(
            name: converted[0].ToLowerInvariant(),
            argument: converted.Length > 1
                          ? Maybe.Some(converted[1])
                          : Maybe.None<string>());

        var response = await _machine.Process(command, token);

        return (response is { Length: > 0 } ? response : "{}")
            ._(Maybe.Some);
    }
}