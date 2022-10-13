using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;

namespace TorteLand.Core.Contracts;

public interface INotebooks
{
    IAsyncEnumerable<Unique<Note>> All(int index, CancellationToken token);

    int Create();

    Task<Either<int, Transaction>> Add(int index, string value, CancellationToken token);
    Task<Either<int, Transaction>> Add(int index, Guid id, bool isRight, CancellationToken token);
    Task Delete(int index, int key, CancellationToken token);
}