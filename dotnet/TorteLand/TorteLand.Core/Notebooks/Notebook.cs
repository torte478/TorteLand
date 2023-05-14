using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SoftwareCraft.Functional;
using TorteLand.Contracts;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Notebooks.Models;
using TorteLand.Extensions;

namespace TorteLand.Core.Notebooks;

internal sealed class Notebook : INotebook
{
    private readonly string[] _values;

    public Notebook(Maybe<IReadOnlyCollection<string>> values)
    {
        _values = values.Match(
                            _ => _,
                            () => ArraySegment<string>.Empty)
                        .ToArray();
    }

    private Notebook(string[] values)
    {
        _values = values;
    }

    public IEnumerator<Unique<Note>> GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Page<Unique<Note>> All(Maybe<Pagination> pagination)
        => Enumerate().Paginate(pagination, _values.Length);

    public Either<AddNotesResult, Segment> Create(
        IReadOnlyCollection<string> values,
        Maybe<ResolvedSegment> segment)
        => _values switch
        {
            { Length: 0 } => AddToEmpty(values),
            _ => AddToExisting(values, segment)
        };

    public Maybe<Note> Read(int key)
        => key >= 0 && key < _values.Length
               ? Maybe.Some(ToNote(key))
               : Maybe.None<Note>(); 

    public INotebook Update(int key, string name)
    {
        var updated = _values.ToArray();
        updated[key] = name;
        return new Notebook(updated);
    }

    public INotebook Delete(int key)
        => _values
           .Where((_, i) => i != key)
           .ToArray()
           ._<Notebook>();

    private Either<AddNotesResult, Segment> AddToExisting(
        IReadOnlyCollection<string> values,
        Maybe<ResolvedSegment> segment)
    {
        var (begin, end) = segment.Match(
            _ => ToHalf(_.Segment, _.IsGreater),
            () => (0, _values.Length));

        if (begin < end)
            return GetNextSegment(begin, end);

        var updated = _values
                      .Take(begin)
                      .Concat(values)
                      .Concat(_values.Skip(begin))
                      .ToArray();

        var notebook = new Notebook(updated);
        var indices = Enumerable
                       .Range(begin, values.Count)
                       .ToArray();
        
        return new AddNotesResult(indices, notebook)
            ._(Either.Left<AddNotesResult, Segment>);
    }

    private static Either<AddNotesResult, Segment> GetNextSegment(int begin, int end)
        => new Segment(
                Begin: begin,
                Border: (begin + end) / 2,
                End: end)
            ._(Either.Right<AddNotesResult, Segment>);

    private static (int, int) ToHalf(Segment segment, bool isRight)
        => isRight
               ? (segment.Begin, segment.Border)
               : (segment.Border + 1, segment.End);

    private static Either<AddNotesResult, Segment> AddToEmpty(
        IReadOnlyCollection<string> values)
    {
        var notebook = new Notebook(values.ToArray());
        var indices = Enumerable
                      .Range(0, values.Count)
                      .ToArray();
        
        return new AddNotesResult(indices, notebook)
               ._(Either.Left<AddNotesResult, Segment>);
    }

    private IEnumerable<Unique<Note>> Enumerate()
    {
        for (var i = 0; i < _values.Length; ++i)
            yield return new Unique<Note>(i, ToNote(i));
    }

    private Note ToNote(int index) 
        => new(_values[index], _values.Length - index - 1);
}