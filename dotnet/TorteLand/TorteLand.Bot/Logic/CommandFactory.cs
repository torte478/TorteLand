using SoftwareCraft.Functional;

namespace TorteLand.Bot.Logic;

internal sealed class CommandFactory : ICommandFactory
{
    public ICommand Create(string input)
    {
        var trimmed = input.Trim();
        var space = trimmed.IndexOf(' ');

        return new Command(
            name: (space > -1
                       ? trimmed[..space]
                       : trimmed)
                .ToLowerInvariant(),

            argument: space > -1
                          ? Maybe.Some(trimmed[(space + 1)..])
                          : Maybe.None<string>());
    }
}