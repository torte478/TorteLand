namespace TorteLand.Bot.Logic;

internal interface ICommand
{
    string Name { get; }

    int GetIntArgument();
    string GetStringArgument();
}