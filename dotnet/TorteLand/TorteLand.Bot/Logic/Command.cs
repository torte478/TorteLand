using System.Collections.Generic;
using System.Linq;

namespace TorteLand.Bot.Logic;

internal sealed class Command : ICommand
{
    private readonly IReadOnlyCollection<string> _tokens;
    public string Name { get; }

    public Command(string name, IReadOnlyCollection<string> tokens)
    {
        Name = name;
        _tokens = tokens;
    }

    public int GetInt(int index)
        => _tokens.Skip(index + 1).First()._(int.Parse);

    public string GetString(int index)
        => _tokens.Skip(index + 1).First();

    public string GetTail(int index)
        => _tokens
           .Skip(index + 1)
           ._(_ => string.Join(' ', _));
}