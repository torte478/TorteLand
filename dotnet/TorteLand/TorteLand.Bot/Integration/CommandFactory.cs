namespace TorteLand.Bot.Integration;

internal sealed class CommandFactory : ICommandFactory
{
    public ICommand Create(string input)
        => new Command(input);
}