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
    private readonly byte _pluses;
    private readonly Item[] _values;

    public Notebook(Maybe<IReadOnlyCollection<(string, byte)>> values, byte pluses)
    {
        _pluses = pluses;
        
        _values = values.Match(
                            _ => _.Select(x => new Item(x.Item1, x.Item2)).ToArray(),
                            Array.Empty<Item>);
    }

    private Notebook(Item[] values, byte pluses)
    {
        _values = values;
        _pluses = pluses;
    }

    public IEnumerator<Unique<Note>> GetEnumerator()
        => Enumerate().GetEnumerator();
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
        updated[key] = updated[key] with { Text = name };
        return new Notebook(updated, _pluses);
    }

    public INotebook Delete(int key)
        => _values
           .Where((_, i) => i != key)
           .ToArray()
           ._(_ => new Notebook(_, _pluses));

    public (INotebook Notebook, Either<byte, int> Result) Increment(int key)
    {
        var old = _values[key].Pluses;

        return old < _pluses - 1
                   ? ChangePlusCount(key, old + 1)
                   : IncreaseNoteIndex(key);
    }
    
    public (INotebook Notebook, Either<byte, int> Result) Decrement(int key)
    {
        var old = _values[key].Pluses;

        return old > 0
                   ? ChangePlusCount(key, old - 1)
                   : DecreaseNoteIndex(key);
    }

    private (INotebook Notebook, Either<byte, int> Result) IncreaseNoteIndex(int key)
    {
        var increased = Math.Max(0, key - _pluses);
        
        var updated = new List<Item>();
        for (var i = 0; i < increased; ++i)
            updated.Add(_values[i]);
        
        updated.Add(_values[key] with { Pluses = 0 });
        
        for (var i = increased; i < _values.Length; ++i)
            if (i != key)
                updated.Add(_values[i]);
        return
            (
                new Notebook(updated.ToArray(), _pluses),
                Either.Right<byte, int>(increased)
            );
    }
    
    private (INotebook Notebook, Either<byte, int> Result) DecreaseNoteIndex(int key)
    {
        var decreased = Math.Min(_values.Length - 1, key + _pluses);
        
        var updated = new List<Item>();
        for (var i = 0; i <= decreased; ++i)
            if (i != key)
                updated.Add(_values[i]);
        
        updated.Add(_values[key] with { Pluses = (byte)(_pluses - 1) });
        
        for (var i = decreased + 1; i < _values.Length; ++i)
                updated.Add(_values[i]);

        return
            (
                new Notebook(updated.ToArray(), _pluses),
                Either.Right<byte, int>(decreased)
            );
    }

    private (INotebook Notebook, Either<byte, int> Result) ChangePlusCount(int key, int pluses)
    {
        var updated = _values.ToArray();
        
        updated[key] = updated[key] with { Pluses = (byte)pluses };

        return
            (
                new Notebook(updated, _pluses),
                Either.Left<byte, int>((byte)pluses)
            );
    }

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
                      .Concat(values.Select(_ => new Item(_)))
                      .Concat(_values.Skip(begin))
                      .ToArray();

        var notebook = new Notebook(updated, _pluses);
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

    private Either<AddNotesResult, Segment> AddToEmpty(
        IReadOnlyCollection<string> values)
    {
        var notebook = new Notebook(
            values.Select(_ => new Item(_)).ToArray(), 
            _pluses);
        
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
    {
        var item = _values[index];
        var pluses = item.Pluses > 0
                         ? Maybe.Some(item.Pluses)
                         : Maybe.None<byte>();

        return new Note(
            Text: item.Text,
            Weight: _values.Length - index - 1,
            Pluses: pluses);
    }

    private sealed record Item(
        string Text,
        byte Pluses = default);
}