using SoftwareCraft.Functional;

namespace TorteLand.Bot.Bot;

internal sealed class CommandFactory : ICommandFactory
{
    public ICommand Create(string name, Maybe<string> argument) => new Command(name, argument);
}