using System;
using System.Linq;

namespace TorteLand.Bot.Logic;

internal sealed class CommandFactory : ICommandFactory
{
    public ICommand Create(string input)
    {
        var lines = input.Trim().Split(new[] { '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);

        var space = lines[0].IndexOf(' ');

        var first = space > -1
                        ? new[] { lines[0][(space + 1)..] }
                        : Enumerable.Empty<string>();

        var name = space > -1 ? lines[0][..space] : lines[0];
        var tokens = first.Concat(lines.Skip(1)).ToArray();

        return new Command(
            name: name.Trim().ToLowerInvariant(),
            lines: tokens);
    }
}