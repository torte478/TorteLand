using System.Collections.Generic;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

namespace TorteLand.Core.Notebooks;

internal interface INotebook : IEnumerable<Unique<Note>>
{
    Either<int, Segment> Add(string value, Maybe<ResolvedSegment> segment);
    Note ToNote(int key);
    INotebook Clone();
    Note Delete(int key);
}