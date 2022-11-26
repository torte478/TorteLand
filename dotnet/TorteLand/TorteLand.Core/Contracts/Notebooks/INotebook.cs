using System.Collections.Generic;
using SoftwareCraft.Functional;

namespace TorteLand.Core.Contracts.Notebooks;

// TODOv2: rename to ACRUD
public interface INotebook : IEnumerable<Unique<Note>>
{
    Page<Unique<Note>> All(Maybe<Pagination> pagination);
    Either<IReadOnlyCollection<int>, Segment> Add(IReadOnlyCollection<string> values, Maybe<ResolvedSegment> segment);
    Note ToNote(int key);
    void Update(int key, string name);
    Note Delete(int key);
    INotebook Clone();
    Maybe<string> Read(int key);
}