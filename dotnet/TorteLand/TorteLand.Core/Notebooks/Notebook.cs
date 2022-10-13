using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Notebooks;

internal sealed class Notebook : INotebook
{
    private readonly List<string> _values;

    public Notebook(Maybe<List<string>> values)
    {
        _values = values.Match(
            _ => _,
            () => new List<string>());
    }

    public IEnumerator<Unique<Note>> GetEnumerator()
        => _values
           .Select((x, i) => new Unique<Note>(i, new Note(x, i)))
           .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Either<int, Segment> Add(string value, Maybe<ResolvedSegment> segment)
        => _values switch
        {
            { Count: 0 } => AddToEmpty(value),
            _ => AddToExisting(value, segment)
        };

    public INotebook Clone()
        => _values
           .ToList()
           ._(Maybe.Some)
           ._(_ => new Notebook(_));

    public Note Delete(int key)
    {
        var deleted = _values[key];
        var updated = _values.Where((_, i) => i != key).ToArray();
        _values.Clear();
        _values.AddRange(updated);

        return new Note(deleted, key);
    }

    public Note ToNote(int key) => new(_values[key], key);

    private Either<int,Segment> AddToExisting(string value, Maybe<ResolvedSegment> segment)
    {
        var (begin, end) = segment.Match(
            _ => ToHalf(_.Segment, _.IsRight),
            () => (0, _values.Count));

        if (begin < end)
            return GetNextSegment(begin, end);

        _values.Add(value);
        for (var i = _values.Count - 1; i > begin; --i)
            _values[i] = _values[i - 1];
        _values[begin] = value;

        return new Left<int, Segment>(begin);
    }

    private static Either<int, Segment> GetNextSegment(int begin, int end)
        => new Segment(
                Begin: begin,
                Border: (begin + end) / 2,
                End: end)
            ._(Either.Right<int, Segment>);

    private static (int, int) ToHalf(Segment segment, bool isRight)
        => isRight
               ? (segment.Border + 1, segment.End)
               : (segment.Begin, segment.Border);

    private Either<int, Segment> AddToEmpty(string value)
    {
        _values.Add(value);
        return new Left<int, Segment>(0);
    }
}