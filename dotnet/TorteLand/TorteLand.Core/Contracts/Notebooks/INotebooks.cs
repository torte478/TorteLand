﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebooks
{
    IAsyncEnumerable<Unique<string>> All(CancellationToken token);
    Task<int> Create(string name, CancellationToken token);
    IAsyncEnumerable<Unique<Note>> Read(int index, CancellationToken token);
    Task<Either<int, Question>> Add(int index, string value, CancellationToken token);
    Task<Either<int, Question>> Add(int index, Guid id, bool isRight, CancellationToken token);
    Task Delete(int index, int key, CancellationToken token);
}