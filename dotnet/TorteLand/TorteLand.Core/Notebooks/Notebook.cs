using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Notebooks;

// TODO: to immutable
internal sealed class Notebook : INotebook
{
    private readonly List<string> _values;

    public Notebook(Maybe<IReadOnlyCollection<string>> values)
    {
        _values = values.Match(
                            _ => _,
                            () => ArraySegment<string>.Empty)
                        ._(_ => new List<string>(_));
    }

    public IEnumerator<Unique<Note>> GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Page<Unique<Note>> All(Maybe<Pagination> pagination)
        => Enumerate().Paginate(pagination, _values.Count);

    public Either<IReadOnlyCollection<int>, Segment> Add(IReadOnlyCollection<string> values, Maybe<ResolvedSegment> segment)
        => _values switch
        {
            { Count: 0 } => AddToEmpty(values),
            _ => AddToExisting(values, segment)
        };

    public INotebook Clone()
        => _values
           .ToArray()
           ._(Maybe.Some<IReadOnlyCollection<string>>)
           ._(_ => new Notebook(_));

    public Maybe<string> Read(int key)
        => key >= 0 && key < _values.Count
               ? Maybe.Some(_values[key])
               : Maybe.None<string>(); 

    public void Update(int key, string name)
    {
        _values[key] = name;
    }

    public Note Delete(int key)
    {
        var deleted = _values[key];
        var updated = _values.Where((_, i) => i != key).ToArray();
        _values.Clear();
        _values.AddRange(updated);

        return new Note(deleted, key);
    }

    public Note ToNote(int key) => new(_values[key], key);

    private Either<IReadOnlyCollection<int>, Segment> AddToExisting(
        IReadOnlyCollection<string> values,
        Maybe<ResolvedSegment> segment)
    {
        var (begin, end) = segment.Match(
            _ => ToHalf(_.Segment, _.IsRight),
            () => (0, _values.Count));

        if (begin < end)
            return GetNextSegment(begin, end);

        var updated = _values
                      .Take(begin)
                      .Concat(values)
                      .Concat(_values.Skip(begin))
                      .ToArray();

        _values.Clear();
        _values.AddRange(updated);

        return Enumerable
               .Range(begin, values.Count)
               .ToArray()
               ._(Either.Left<IReadOnlyCollection<int>, Segment>);
    }

    private static Either<IReadOnlyCollection<int>, Segment> GetNextSegment(int begin, int end)
        => new Segment(
                Begin: begin,
                Border: (begin + end) / 2,
                End: end)
            ._(Either.Right<IReadOnlyCollection<int>, Segment>);

    private static (int, int) ToHalf(Segment segment, bool isRight)
        => isRight
               ? (segment.Begin, segment.Border)
               : (segment.Border + 1, segment.End);

    private Either<IReadOnlyCollection<int>, Segment> AddToEmpty(IReadOnlyCollection<string> values)
    {
        _values.AddRange(values);
        return Enumerable
               .Range(0, values.Count)
               .ToArray()
               ._(Either.Left<IReadOnlyCollection<int>, Segment>);
    }

    private IEnumerable<Unique<Note>> Enumerate()
    {
        for (var i = 0; i < _values.Count; ++i)
            yield return new Unique<Note>(i, new Note(_values[i], _values.Count - i - 1));
    }
}