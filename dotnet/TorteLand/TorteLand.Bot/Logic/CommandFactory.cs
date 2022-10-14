using System;
using System.Linq;

namespace TorteLand.Bot.Logic;

internal sealed class CommandFactory : ICommandFactory
{
    public ICommand Create(string input)
    {
        var byLine = input.Trim().Split(new[] { '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);
        var tokens = byLine[0].Split(' ').Concat(byLine.Skip(1)).ToArray();

        return new Command(
            name: tokens[0].Trim().ToLowerInvariant(),
            tokens: tokens);
    }
}