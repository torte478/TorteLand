namespace TorteLand.Bot.Logic;

internal interface ICommandFactory
{
    ICommand Create(string raw);
}