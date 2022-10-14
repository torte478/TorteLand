using System;
using SoftwareCraft.Functional;

namespace TorteLand.Bot.Logic;

internal sealed class Command : ICommand
{
    private readonly Maybe<string> _argument;
    public string Name { get; }

    public Command(string name, Maybe<string> argument)
    {
        Name = name;
        _argument = argument;
    }

    public int GetIntArgument()
        => _argument.Match(
            int.Parse,
            () => throw new Exception("Argument is none"));

    public string GetStringArgument()
        => _argument.Match(
            _ => _,
            () => throw new Exception("Argument is none"));
}