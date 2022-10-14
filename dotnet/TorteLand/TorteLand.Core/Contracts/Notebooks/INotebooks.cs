using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebooks
{
    IAsyncEnumerable<Unique<Note>> Read(int index, CancellationToken token);
    Task<Either<int, Question>> Add(int index, string value, CancellationToken token);
    Task<Either<int, Question>> Add(int index, Guid id, bool isRight, CancellationToken token);
    Task Rename(int index, int id, string text, CancellationToken token);
    Task Delete(int index, int key, CancellationToken token);
}