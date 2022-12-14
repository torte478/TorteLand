using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebooks
{
    Task<Page<Unique<Note>>> All(int index, Maybe<Pagination> pagination, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, IReadOnlyCollection<string> values, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, Guid id, bool isRight, CancellationToken token);
    Task<Maybe<string>> Read(int index, int key, CancellationToken token);
    Task Update(int index, int id, string name, CancellationToken token);
    Task Delete(int index, int id, CancellationToken token);
}