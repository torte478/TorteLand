using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Contracts;
using TorteLand.Core.Contracts.Notebooks.Models;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebooks
{
    Task<Page<Unique<Note>>> All(int index, Maybe<Pagination> pagination, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, IReadOnlyCollection<string> values, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, Guid id, bool isRight, CancellationToken token);
    Task<Maybe<Note>> Read(int index, int id, CancellationToken token);
    Task Update(int index, int id, string name, CancellationToken token);
    Task Delete(int index, int id, CancellationToken token);
    Task<Either<byte, int>> Increment(int index, int id, CancellationToken token);
}