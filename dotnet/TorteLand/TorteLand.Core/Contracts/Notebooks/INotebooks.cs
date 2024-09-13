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
    Task<Page<Unique<Note>>> All(int notebookId, Maybe<Pagination> pagination, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(int notebookId, Added added, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(int notebookId, Guid id, bool isRight, CancellationToken token);
    Task<Maybe<Note>> Read(int notebookId, int id, CancellationToken token);
    Task Update(int notebookId, int id, string name, CancellationToken token);
    Task Delete(int notebookId, int id, CancellationToken token);
    Task<Either<byte, int>> Increment(int notebookId, int id, CancellationToken token);
    Task<Either<byte, int>> Decrement(int notebookId, int id, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Actualize(int notebookId, int valueId, CancellationToken token);
}