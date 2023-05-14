using System.Collections.Generic;
using SoftwareCraft.Functional;
using TorteLand.Contracts;
using TorteLand.Core.Contracts.Notebooks.Models;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebook : IEnumerable<Unique<Note>>
{
    Page<Unique<Note>> All(Maybe<Pagination> pagination);
    Either<AddNotesResult, Segment> Create(IReadOnlyCollection<string> values, Maybe<ResolvedSegment> segment);
    Maybe<Note> Read(int key);
    INotebook Update(int key, string name);
    INotebook Delete(int key);
    (INotebook Notebook, Either<byte, int> Result) Increment(int key);
}