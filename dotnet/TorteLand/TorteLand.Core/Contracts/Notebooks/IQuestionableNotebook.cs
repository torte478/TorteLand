using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Notebooks;

public interface IQuestionableNotebook
{
    IAsyncEnumerable<Unique<Note>> All(CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(IReadOnlyCollection<string> values, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(Guid id, bool isRight, CancellationToken token);
    Task Delete(int key, CancellationToken token);
    Task DeleteAll(CancellationToken token);
    Task Rename(int key, string text, CancellationToken token);
}