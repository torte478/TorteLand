using System.Collections.Generic;
using SoftwareCraft.Functional;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebook : IEnumerable<Unique<Note>>
{
    Either<int, Segment> Add(string value, Maybe<ResolvedSegment> segment);
    Note ToNote(int key);
    void Rename(int key, string text);
    Note Delete(int key);
    INotebook Clone();
}