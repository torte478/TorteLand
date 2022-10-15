namespace TorteLand.Bot.Logic;

internal sealed class CommandFactory : ICommandFactory
{
    public ICommand Create(string input)
        => new Command(input);
}