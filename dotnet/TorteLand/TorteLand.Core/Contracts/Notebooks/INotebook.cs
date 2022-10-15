using System.Collections.Generic;
using SoftwareCraft.Functional;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebook : IEnumerable<Unique<Note>>
{
    Page<Unique<Note>> All(Maybe<Pagination> pagination);
    Either<IReadOnlyCollection<int>, Segment> Add(IReadOnlyCollection<string> values, Maybe<ResolvedSegment> segment);
    Note ToNote(int key);
    void Rename(int key, string text);
    Note Delete(int key);
    INotebook Clone();
}