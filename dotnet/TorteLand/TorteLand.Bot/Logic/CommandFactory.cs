using System;

namespace TorteLand.Bot.Logic;

internal sealed class CommandFactory : ICommandFactory
{
    public ICommand Create(string input)
    {
        var tokens = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return new Command(
            name: tokens[0].ToLowerInvariant(),
            tokens: tokens);
    }
}