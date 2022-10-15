using System;
using System.Collections.Generic;
using System.Linq;

namespace TorteLand.Bot.Logic;

internal sealed class Command : ICommand
{
    private readonly string _raw;

    public Command(string raw)
    {
        _raw = raw.Trim();
    }

    public (string Name, ICommand Tail) ToName()
    {
        var _ = Cut();
        return (_.Head.Trim().ToLowerInvariant(), new Command(_.Tail));
    }

    public (string Word, ICommand Tail) ToWord()
    {
        var _ = Cut();
        return (_.Head, new Command(_.Tail));
    }

    public (int Value, ICommand Tail) ToInt()
    {
        var _ = Cut();

        return (_.Head._(int.Parse), new Command(_.Tail));
    }

    public (string Line, ICommand Tail) ToLine()
    {
        var lines = ToLines();
        var first = lines.First();

        var tail = first.Length == _raw.Length
                       ? string.Empty
                       : _raw[(first.Length + 1)..];

        return (first, new Command(tail));
    }

    public IReadOnlyCollection<string> ToLines()
        => _raw.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

    private (string Head, string Tail) Cut()
    {
        var space = new[] { ' ', '\r', '\n' }
                       .Select(_ => _raw.IndexOf(_))
                       .Select(_ => _ == -1 ? int.MaxValue : _)
                       .Min();

        var head = space == int.MaxValue ? _raw : _raw[..space];
        var tail = space == int.MaxValue ? string.Empty : _raw[(space + 1)..];

        return (head, tail);
    }
}