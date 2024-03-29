﻿using System;
using System.Collections.Generic;
using SoftwareCraft.Functional;
using TorteLand.Contracts;
using TorteLand.Core.Contracts.Notebooks.Models;

namespace TorteLand.Core.Contracts.Notebooks;

public interface IQuestionableNotebook : IEnumerable<Unique<Note>>
{
    Page<Unique<Note>> All(Maybe<Pagination> pagination);
    AddNotesIteration Create(Added added);
    AddNotesIteration Create(Guid id, bool isRight);
    Maybe<Note> Read(int key);
    IQuestionableNotebook Update(int key, string name);
    IQuestionableNotebook Delete(int key);
    (IQuestionableNotebook Notebook, Either<byte, int> Result) Increment(int key);
    (IQuestionableNotebook Notebook, Either<byte, int> Result) Decrement(int key);
}