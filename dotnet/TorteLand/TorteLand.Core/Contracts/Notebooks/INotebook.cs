using System.Collections.Generic;
using SoftwareCraft.Functional;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebook : IEnumerable<Unique<Note>>
{
    Either<int, Segment> Add(string value, Maybe<ResolvedSegment> segment);
    Note ToNote(int key);
    INotebook Clone();
    Note Delete(int key);
}