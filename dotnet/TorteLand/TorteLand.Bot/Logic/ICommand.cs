using System.Collections.Generic;

namespace TorteLand.Bot.Logic;

internal interface ICommand
{
    string Name { get; }

    int GetInt(int index = 0);
    string GetLine(int index = 0);
    IReadOnlyCollection<string> GetLines(int index = 0);
}