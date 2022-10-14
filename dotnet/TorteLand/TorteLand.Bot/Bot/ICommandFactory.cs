using SoftwareCraft.Functional;

namespace TorteLand.Bot.Bot;

internal interface ICommandFactory
{
    ICommand Create(string name, Maybe<string> argument);
}