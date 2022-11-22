﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Notebooks;

// TODO : duplicates IAsyncNotebook, remove async
public interface IQuestionableNotebook
{
    Task<Page<Unique<Note>>> All(Maybe<Pagination> pagination, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(IReadOnlyCollection<string> values, CancellationToken token);
    Task<Either<IReadOnlyCollection<int>, Question>> Add(Guid id, bool isRight, CancellationToken token);
    Task Delete(int key, CancellationToken token);
    Task DeleteAll(CancellationToken token);
    Task Update(int key, string name, CancellationToken token);
    Task<Maybe<string>> Read(int key, CancellationToken token);
}