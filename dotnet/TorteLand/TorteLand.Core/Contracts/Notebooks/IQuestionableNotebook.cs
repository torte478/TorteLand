using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Notebooks;

// TODO : duplicates IAsyncNotebook
public interface IQuestionableNotebook
{
    Task<Page<Unique<Note>>> All(Maybe<Pagination> pagination, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(IReadOnlyCollection<string> values, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(Guid id, bool isRight, CancellationToken token);
    Task Delete(int key, CancellationToken token);
    Task DeleteAll(CancellationToken token);
    Task Rename(int key, string text, CancellationToken token);
}