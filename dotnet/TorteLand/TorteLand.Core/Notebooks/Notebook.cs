using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

namespace TorteLand.Core.Notebooks;

internal sealed class Notebook<T> : INotebook<int, T>
{
    private readonly List<T> _values;

    public Notebook()
    {
        _values = new List<T>();
    }

    public IEnumerator<(int, T)> GetEnumerator()
        => _values.Select((x, i) => (i, x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Either<int, Segment<int>> Add(T value, Maybe<HalfSegment<int>> segment)
        => _values switch
        {
            { Count: 0 } => AddToEmpty(value),
            _ => AddToExisting(value, segment)
        };

    private Either<int,Segment<int>> AddToExisting(T value, Maybe<HalfSegment<int>> segment)
    {
        var (begin, end) = segment.Match(
            _ => ToHalf(_.Segment, _.IsRight),
            () => (0, _values.Count));

        if (begin < end)
            return GetNextSegment(begin, end);

        _values.Add(value);
        for (var i = _values.Count - 1; i > begin; ++i)
            _values[i] = _values[i - 1];
        _values[begin] = value;

        return Either.Left<int, Segment<int>>(begin);
    }

    private static Either<int, Segment<int>> GetNextSegment(int begin, int end)
        => new Segment<int>(
                Begin: begin,
                Middle: (begin + end) / 2,
                End: end)
            ._(Either.Right<int, Segment<int>>);

    private static (int, int) ToHalf(Segment<int> segment, bool isRight)
        => isRight
               ? (segment.Middle, segment.End)
               : (segment.Begin, segment.Middle);

    private Either<int,Segment<int>> AddToEmpty(T value)
    {
        _values.Add(value);
        return Either.Left<int, Segment<int>>(0);
    }
}