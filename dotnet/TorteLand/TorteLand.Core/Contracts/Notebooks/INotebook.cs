using System.Collections.Generic;
using SoftwareCraft.Functional;
using TorteLand.Contracts;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebook : IEnumerable<Unique<Note>>
{
    Page<Unique<Note>> All(Maybe<Pagination> pagination);
    Either<AddNotesResult, Segment> Create(IReadOnlyCollection<string> values, Maybe<ResolvedSegment> segment);
    Maybe<Note> Read(int key);
    INotebook Update(int key, string name);
    INotebook Delete(int key);
}