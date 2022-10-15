using System.Collections.Generic;
using System.Linq;

namespace TorteLand.Bot.Logic;

// TODO: refactor command logic
internal sealed class Command : ICommand
{
    private readonly IReadOnlyCollection<string> _lines;
    public string Name { get; }

    public Command(string name, IReadOnlyCollection<string> lines)
    {
        Name = name;
        _lines = lines;
    }

    public int GetInt(int index)
        => GetLine(0).Split(' ')[index]._(int.Parse);

    public string GetLine(int index)
        => _lines.Count > 1
               ? _lines.Skip(index).First()
               : _lines.First().Split(' ')[index];

    public IReadOnlyCollection<string> GetLines(int index)
        => _lines
           .Skip(index)
           .ToArray();
}