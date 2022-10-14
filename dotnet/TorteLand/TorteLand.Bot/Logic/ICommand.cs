namespace TorteLand.Bot.Logic;

internal interface ICommand
{
    string Name { get; }

    int GetInt(int index = 0);
    string GetString(int index = 0);
    string GetTail(int index = 0);
}