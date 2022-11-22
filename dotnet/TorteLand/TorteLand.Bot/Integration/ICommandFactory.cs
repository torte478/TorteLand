namespace TorteLand.Bot.Integration;

internal interface ICommandFactory
{
    ICommand Create(string raw);
}