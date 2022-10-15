using System.Collections.Generic;

namespace TorteLand.Bot.Logic;

internal interface ICommand
{
    (string Name, ICommand Tail) ToName();
    (string Word, ICommand Tail) ToWord();
    (int Value, ICommand Tail) ToInt();
    (string Line, ICommand Tail) ToLine();
    IReadOnlyCollection<string> ToLines();
}