using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;

namespace TorteLand.Core.Notebooks;

internal interface ITransactionNotebook
{
    IAsyncEnumerable<Unique<Note>> All(CancellationToken token);
    Task<Either<int, Transaction>> Add(string value, CancellationToken token);
    Task<Either<int, Transaction>> Add(Guid id, bool isRight, CancellationToken token);
    Task Delete(int key, CancellationToken token);
}