using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Notebooks;

public interface IPersistedNotebook
{
    Task<Page<Unique<Note>>> All(Maybe<Pagination> pagination, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Create(IReadOnlyCollection<string> values, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Create(Guid id, bool isRight, CancellationToken token);
    Task<Maybe<string>> Read(int key, CancellationToken token);
    Task Update(int key, string name, CancellationToken token);
    Task Delete(int key, CancellationToken token);
}