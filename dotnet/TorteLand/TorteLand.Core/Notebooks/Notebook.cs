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
                            _ => _.Select(x => new Item(x.Item1, x.Item2)).ToArray(), // TODO .Item ?
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
        Added added,
        Maybe<ResolvedSegment> segment)
        => _values switch
        {
            { Length: 0 } => AddToEmpty(added.Values),
            _ => AddToExisting(added, segment)
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
        Added added,
        Maybe<ResolvedSegment> segment)
        => GetInsertPosition(added, segment)
            .Match(
                index => InsertValue(index, added),
                Either.Right<AddNotesResult, Segment>);

    private Either<AddNotesResult, Segment> InsertValue(int index, Added added)
    {
        var updated = _values
                      .Take(index)
                      .Concat(added.Values.Select(_ => new Item(_)))
                      .Concat(_values.Skip(index))
                      .ToArray();

        var notebook = new Notebook(updated, _pluses);
        var indices = Enumerable
                      .Range(index, added.Values.Count)
                      .ToArray();

        return new AddNotesResult(indices, notebook)
            ._(Either.Left<AddNotesResult, Segment>);
    }

    private Either<int, Segment> GetInsertPosition(Added added, Maybe<ResolvedSegment> segment)
    {
        if (added.Exact)
            return GetExactPosition(added);

        var result = GetSegment(added, segment);
        
        return result.Begin == result.End
                   ? Either.Left<int, Segment>(result.Begin)
                   : Either.Right<int, Segment>(result);
    }

    private Segment GetSegment(Added added, Maybe<ResolvedSegment> segment)
    {
        if (segment.IsNone)
            return ToStartSegment(added);
        
        var resolved = segment.ToSome();
        return resolved.IsGreater
                         ? new Segment(resolved.Segment.Begin, resolved.Segment.Border)
                         : new Segment(resolved.Segment.Border + 1, resolved.Segment.End);
    }

    private static Either<int, Segment> GetExactPosition(Added added)
    {
        var origin = added.Origin.ToSome();
        var index = added.Direction == Direction.After
                        ? origin + 1
                        : origin;

        return index._(Either.Left<int, Segment>);
    }

    private Segment ToStartSegment(Added added)
        => added.Origin.Match(
            origin => added.Direction switch
            {
                Direction.After => new Segment(origin + 1, _values.Length),
                _ => new Segment(0, origin)
            },
            () => new Segment(0, _values.Length));
    
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