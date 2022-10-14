using System.Threading;

namespace TorteLand.Bot.Bot;

internal interface ICommand
{
    string Name { get; }

    int GetIntArgument();
    string GetStringArgument();
}