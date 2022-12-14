using System;
using System.Collections.Generic;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Notebooks;

public interface IQuestionableNotebook : IEnumerable<Unique<Note>>
{
    Page<Unique<Note>> All(Maybe<Pagination> pagination);
    Either<IReadOnlyCollection<int>, Question> Create(IReadOnlyCollection<string> values);
    Either<IReadOnlyCollection<int>, Question> Create(Guid id, bool isRight);
    Maybe<string> Read(int key);
    void Update(int key, string name);
    void Delete(int key);

    IQuestionableNotebook Clone();
}